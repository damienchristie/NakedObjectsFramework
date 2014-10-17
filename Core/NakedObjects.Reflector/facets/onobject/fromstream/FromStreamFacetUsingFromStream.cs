// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.IO;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Capabilities;
using NakedObjects.Metamodel.Facet;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.FromStream {
    public class FromStreamFacetUsingFromStream : FacetAbstract, IFromStreamFacet {
        private readonly IFromStream fromStream;


        public FromStreamFacetUsingFromStream(IFromStream fromStream, ISpecification holder)
            : base(typeof (IFromStreamFacet), holder) {
            this.fromStream = fromStream;
        }

        #region IFromStreamFacet Members

        public INakedObject ParseFromStream(Stream stream, string mimeType, string name, INakedObjectManager manager) {
            object obj = fromStream.ParseFromStream(stream, mimeType, name);
            return manager.CreateAdapter(obj, null, null);
        }

        #endregion

        protected override string ToStringValues() {
            return fromStream.ToString();
        }
    }
}