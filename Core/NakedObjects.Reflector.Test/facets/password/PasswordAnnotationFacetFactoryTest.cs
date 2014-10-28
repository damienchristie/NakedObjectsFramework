// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.FacetFactory;
using NakedObjects.Metamodel.Facet;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Password {
    [TestFixture]
    public class PasswordAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new PasswordAnnotationFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private PasswordAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IPasswordFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        private class Customer1 {
            [DataType(DataType.Password)]
            public string FirstName {
                get { return null; }
            }
        }

        private class Customer2 {
            public void someAction([DataType(DataType.Password)] string foo) {}
        }

        private class Customer3 {
            [DataType(DataType.PhoneNumber)]
            public string FirstName {
                get { return null; }
            }
        }

        private class Customer4 {
            public void someAction([DataType(DataType.PhoneNumber)] string foo) {}
        }


        [Test]
        public override void TestFeatureTypes() {
            FeatureType featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(featureTypes.HasFlag( FeatureType.Objects));
            Assert.IsTrue(featureTypes.HasFlag( FeatureType.Property));
            Assert.IsFalse(featureTypes.HasFlag( FeatureType.Collections));
            Assert.IsFalse(featureTypes.HasFlag( FeatureType.Action));
            Assert.IsTrue(featureTypes.HasFlag( FeatureType.ActionParameter));
        }

        [Test]
        public void TestPasswordAnnotationNotPickedUpOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer4), "someAction", new[] {typeof (string)});
            facetFactory.ProcessParams(method, 0, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPasswordFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public void TestPasswordAnnotationNotPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer3), "FirstName");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPasswordFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public void TestPasswordAnnotationPickedUpOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer2), "someAction", new[] {typeof (string)});
            facetFactory.ProcessParams(method, 0, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPasswordFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PasswordFacet);
        }

        [Test]
        public void TestPasswordAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer1), "FirstName");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPasswordFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PasswordFacet);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}