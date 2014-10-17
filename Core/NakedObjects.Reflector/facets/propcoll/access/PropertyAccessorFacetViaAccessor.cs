// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;
using NakedObjects.Metamodel.Facet;
using NakedObjects.Reflector.DotNet.Reflect.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Propcoll.Access {
    public class PropertyAccessorFacetViaAccessor : PropertyAccessorFacetAbstract, IImperativeFacet {
        private readonly PropertyInfo propertyMethod;

        public PropertyAccessorFacetViaAccessor(PropertyInfo property, ISpecification holder)
            : base(holder) {
            propertyMethod = property;
        }

        #region IImperativeFacet Members

        public MethodInfo GetMethod() {
            return propertyMethod.GetGetMethod();
        }

        #endregion

        public override object GetProperty(INakedObject nakedObject) {
            try {
                return propertyMethod.GetValue(nakedObject.GetDomainObject(), null);
            }
            catch (TargetInvocationException e) {
                InvokeUtils.InvocationException("Exception executing " + propertyMethod, e);
                return null;
            }
        }

        protected override string ToStringValues() {
            return "propertyMethod=" + propertyMethod;
        }
    }
}