// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Reflection;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.FacetFactory;
using NakedObjects.Metamodel.Facet;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Properties.Eagerly {
    [TestFixture]
    public class EagerlyAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            annotationFacetFactory = new EagerlyAnnotationFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            annotationFacetFactory = null;
            base.TearDown();
        }

        #endregion

        private EagerlyAnnotationFacetFactory annotationFacetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IEagerlyFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return annotationFacetFactory; }
        }

        private class Customer1 {
            [Eagerly(EagerlyAttribute.Do.Rendering)]
            public int Prop { get; set; }

            [Eagerly(EagerlyAttribute.Do.Rendering)]
            public IList<Customer1> Coll { get; set; }

            [Eagerly(EagerlyAttribute.Do.Rendering)]
            public IList<Customer1> Act() {
                return new List<Customer1>();
            }
        }

        [Eagerly(EagerlyAttribute.Do.Rendering)]
        private class Customer2 {
            public int Prop { get; set; }

            public IList<Customer1> Coll { get; set; }

            public IList<Customer1> Act() {
                return new List<Customer1>();
            }
        }

        [Test]
        public void TestEagerlyAnnotationPickedUpOnClass() {
            annotationFacetFactory.Process(typeof (Customer2), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IEagerlyFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is EagerlyFacet);
            var propertyDefaultFacetAnnotation = (EagerlyFacet) facet;
            Assert.AreEqual(EagerlyAttribute.Do.Rendering, propertyDefaultFacetAnnotation.What);
        }

        [Test]
        public void TestEagerlyAnnotationPickedUpOnCollection() {
            PropertyInfo property = FindProperty(typeof (Customer1), "Coll");
            annotationFacetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IEagerlyFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is EagerlyFacet);
            var propertyDefaultFacetAnnotation = (EagerlyFacet) facet;
            Assert.AreEqual(EagerlyAttribute.Do.Rendering, propertyDefaultFacetAnnotation.What);
        }

        [Test]
        public void TestEagerlyAnnotationPickedUpOnMethod() {
            MethodInfo method = FindMethod(typeof (Customer1), "Act");
            annotationFacetFactory.Process(method, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IEagerlyFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is EagerlyFacet);
            var propertyDefaultFacetAnnotation = (EagerlyFacet) facet;
            Assert.AreEqual(EagerlyAttribute.Do.Rendering, propertyDefaultFacetAnnotation.What);
        }

        [Test]
        public void TestEagerlyAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer1), "Prop");
            annotationFacetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IEagerlyFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is EagerlyFacet);
            var propertyDefaultFacetAnnotation = (EagerlyFacet) facet;
            Assert.AreEqual(EagerlyAttribute.Do.Rendering, propertyDefaultFacetAnnotation.What);
        }

        [Test]
        public void TestEagerlyNotPickedUpOnClass() {
            annotationFacetFactory.Process(typeof (Customer1), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IEagerlyFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public void TestEagerlyNotPickedUpOnCollection() {
            PropertyInfo property = FindProperty(typeof (Customer2), "Coll");
            annotationFacetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IEagerlyFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public void TestEagerlyNotPickedUpOnMethod() {
            MethodInfo method = FindMethod(typeof (Customer2), "Act");
            annotationFacetFactory.Process(method, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IEagerlyFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public void TestEagerlyNotPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer2), "Prop");
            annotationFacetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IEagerlyFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public override void TestFeatureTypes() {
            FeatureType featureTypes = annotationFacetFactory.FeatureTypes;
            Assert.IsTrue(featureTypes.HasFlag( FeatureType.Objects));
            Assert.IsTrue(featureTypes.HasFlag( FeatureType.Property));
            Assert.IsTrue(featureTypes.HasFlag( FeatureType.Collections));
            Assert.IsTrue(featureTypes.HasFlag( FeatureType.Action));
            Assert.IsFalse(featureTypes.HasFlag( FeatureType.ActionParameter));
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}