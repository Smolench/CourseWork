using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;
using System.IO;
using CourseWork.Models;

namespace CourseWork
{
    public static class LuceneSearch<T> where T:new()
    {
        private static string luceneDirComment = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "lucene_index\\comment");

        private static string luceneDirText = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "lucene_index\\text");
        private static FSDirectory directoryTemp;
        private static FSDirectory directory
        {
            get
            {
                string luceneDir = "";
                if (typeof(T).Equals(typeof(Comment)))
                    luceneDir = luceneDirComment;
                else
                    luceneDir = luceneDirText;
                if (directoryTemp == null)
                    directoryTemp = FSDirectory.Open(new DirectoryInfo(luceneDir));
                if (IndexWriter.IsLocked(directoryTemp))
                    IndexWriter.Unlock(directoryTemp);
                var lockFilePath = Path.Combine(luceneDir, "write.lock");
                if (File.Exists(lockFilePath))
                    File.Delete(lockFilePath);
                return directoryTemp;
            }
        }

        private static void AddToLuceneIndex(T content, IndexWriter writer)
        {
            // remove older index entry

            writer.DeleteAll();
            // add new index entry
            var doc = new Document();

            // add lucene fields mapped to db fields
            doc.Add(new Field("Id", typeof(T).GetProperty("Id").GetValue(content).ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Description", typeof(T).GetProperty("Description").GetValue(content).ToString(), Field.Store.YES, Field.Index.ANALYZED));

            // add entry to index
            writer.AddDocument(doc);
        }

        public static void AddUpdateLuceneIndex(IEnumerable<T> contents)
        {
            // init lucene
            var analyzer = new StandardAnalyzer(Version.LUCENE_30);
            using (var writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                // add data to lucene search index (replaces older entry if any)
                foreach (var content in contents) AddToLuceneIndex(content, writer);

                // close handles
                analyzer.Close();
                writer.Dispose();
            }
        }

        public static void AddUpdateLuceneIndex(T contnent)
        {
            AddUpdateLuceneIndex(new List<T> { contnent });
        }

        public static void ClearLuceneIndexRecord(int record_id)
        {
            // init lucene
            var analyzer = new StandardAnalyzer(Version.LUCENE_30);
            using (var writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                // remove older index entry
                var searchQuery = new TermQuery(new Term("Id", record_id.ToString()));
                writer.DeleteDocuments(searchQuery);

                // close handles
                analyzer.Close();
                writer.Dispose();
            }
        }

        public static bool ClearLuceneIndex()
        {
            try
            {
                var analyzer = new StandardAnalyzer(Version.LUCENE_30);
                using (var writer = new IndexWriter(directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    // remove older index entries
                    writer.DeleteAll();

                    // close handles
                    analyzer.Close();
                    writer.Dispose();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static void Optimize()
        {
            var analyzer = new StandardAnalyzer(Version.LUCENE_30);
            using (var writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                analyzer.Close();
                writer.Optimize();
                writer.Dispose();
            }
        }

        private static T MapLuceneDocumentToData(Document doc)
        {
            T newMap = new T();
            typeof(T).GetProperty("Id").SetValue(newMap, Convert.ToInt32(doc.Get("Id")));
            typeof(T).GetProperty("Description").SetValue(newMap, doc.Get("Description"));
            return newMap;
        }

        private static IEnumerable<T> MapLuceneToDataList(IEnumerable<Document> hits)
        {
            return hits.Select(MapLuceneDocumentToData).ToList();
        }
        private static IEnumerable<T> MapLuceneToDataList(IEnumerable<ScoreDoc> hits,
            IndexSearcher searcher)
        {
            return hits.Select(hit => MapLuceneDocumentToData(searcher.Doc(hit.Doc))).ToList();
        }

        private static Query ParseQuery(string searchQuery, QueryParser parser)
        {
            Query query;
            try
            {
                query = parser.Parse(searchQuery.Trim());
            }
            catch (ParseException)
            {
                query = parser.Parse(QueryParser.Escape(searchQuery.Trim()));
            }
            return query;
        }

        private static IEnumerable<T> Search (string searchQuery, string searchField = "")
        {
            // validation
            if (string.IsNullOrEmpty(searchQuery.Replace("*", "").Replace("?", ""))) return new List<T>();

            // set up lucene searcher
            using (var searcher = new IndexSearcher(directory, false))
            {
                var hits_limit = 1000;
                var analyzer = new StandardAnalyzer(Version.LUCENE_30);

                // search by single field
                if (!string.IsNullOrEmpty(searchField))
                {
                    var parser = new QueryParser(Version.LUCENE_30, searchField, analyzer);
                    var query = ParseQuery(searchQuery, parser);
                    var hits = searcher.Search(query, hits_limit).ScoreDocs;
                    var results = MapLuceneToDataList(hits, searcher);
                    analyzer.Close();
                    searcher.Dispose();
                    return results;
                }
                // search by multiple fields (ordered by RELEVANCE)
                else
                {
                    var parser = new MultiFieldQueryParser
                        (Version.LUCENE_30, new[] { "Id", "Description" }, analyzer);
                    var query = ParseQuery(searchQuery, parser);
                    var hits = searcher.Search
                    (query, null, hits_limit, Sort.RELEVANCE).ScoreDocs;
                    var results = MapLuceneToDataList(hits, searcher);
                    analyzer.Close();
                    searcher.Dispose();
                    return results;
                }
            }


        }
        public static IEnumerable<T> GlobalSearch(string input, string fieldName = "")
        {
            if (string.IsNullOrEmpty(input)) return new List<T>();

            var terms = input.Trim().Replace("-", " ").Split(' ')
                .Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim() + "*");
            input = string.Join(" ", terms);

            return Search(input, fieldName);
        }

        public static IEnumerable<T> GlobalSearchDefault(string input, string fieldName = "")
        {
            return string.IsNullOrEmpty(input) ? new List<T>() : Search(input, fieldName);
        }

    }
}