﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.CadPlus.Batch.Extensions.Services;
using Xarial.CadPlus.Batch.Extensions.ViewModels;
using Xarial.XCad;
using Xarial.XCad.Documents;
using Xarial.XCad.Documents.Extensions;
using Xarial.XToolkit;

namespace Xarial.CadPlus.Batch.Extensions.Models
{
    public class ReferenceExtractor
    {
        private readonly string[] m_DrwExtensions;

        private readonly IXApplication m_App;

        public ReferenceExtractor(IXApplication app, string[] drwExtensions) 
        {
            m_App = app;
            m_DrwExtensions = drwExtensions;
        }

        public IXDocument[] GetAllReferences(IXDocument[] docs, ReferencesScope_e scope) 
        {
            if (scope == ReferencesScope_e.SourceDocumentsOnly) 
            {
                return docs;
            }
            
            var allRefs = docs.ToDictionary(
                d => d.Path,
                d => d,
                StringComparer.CurrentCultureIgnoreCase);

            foreach (var doc in docs) 
            {
                IXDocument3D[] deps = null;

                switch (scope) 
                {
                    case ReferencesScope_e.TopLevelReferences:
                        deps = doc.Dependencies;
                        break;

                    case ReferencesScope_e.AllReferences:
                        deps = doc.GetAllDependencies().ToArray();
                        break;

                    default:
                        throw new NotSupportedException();
                }

                foreach (var dep in deps) 
                {
                    if (!allRefs.ContainsKey(dep.Path)) 
                    {
                        allRefs.Add(dep.Path, dep);
                    }
                }
            }

            return allRefs.Values.ToArray();
        }

        public Dictionary<IXDocument, IXDrawing[]> FindAllDrawings(
            IXDocument[] docs, string[] additionalFolders, Action<double> prgHandler)
        {
            var workDirs = GetDocumentsWorkingDirectories(docs);

            var res = new Dictionary<IXDocument, IXDrawing[]>(new DocumentComparer());

            var searchDrawings = new List<string>();

            foreach (var searchDir in FileSystemUtils.GetTopFolders(
                workDirs.Union(additionalFolders ?? new string[0])))
            {
                foreach (var filter in m_DrwExtensions)
                {
                    searchDrawings.AddRange(Directory.GetFiles(searchDir, filter,
                        SearchOption.AllDirectories));
                }
            }

            for (int i = 0; i < searchDrawings.Count; i++)
            {
                var drwFile = searchDrawings[i];

                var drw = m_App.Documents.PreCreate<IXDrawing>();
                drw.Path = drwFile;

                var usedDocs = drw.Dependencies.Intersect(docs,
                    new DocumentComparer());

                if (usedDocs.Any())
                {
                    foreach (var usedDoc in usedDocs)
                    {
                        List<IXDrawing> drwsList;

                        if (res.TryGetValue(usedDoc, out IXDrawing[] drws))
                        {
                            drwsList = new List<IXDrawing>(drws);
                        }
                        else
                        {
                            drwsList = new List<IXDrawing>();
                        }

                        drwsList.Add(drw);
                        res[usedDoc] = drwsList.ToArray();
                    }
                }

                prgHandler.Invoke((double)(i + 1) / (double)searchDrawings.Count);
            }

            return res;
        }

        private string[] GetDocumentsWorkingDirectories(IXDocument[] docs)
            => docs.Select(d => Path.GetDirectoryName(d.Path))
            .Distinct(StringComparer.CurrentCultureIgnoreCase).ToArray();
    }
}
