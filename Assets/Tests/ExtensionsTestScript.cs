using NUnit.Framework;

using UnityEngine;

namespace Tests
{
    public class ExtensionsTestScript
    {
        [Test]
        public void BasicExtensionsTest()
        {
            int num = 5;
            Assert.IsTrue(num.IsAnyOf(1,2,3,4,5,6,7,8,9,10));

            
            int num2 = 11;
            Assert.IsTrue(num2.IsNoneOf(1,2,3,4,5,6,7,8,9,10));

            Extensions.Swap(ref num, ref num2);

            Assert.AreEqual(num2, 5);
            Assert.AreEqual(num, 11);
        }

        [Test]
        public void MathExtensionsTest()
        {
            Vector2 a = Vector2.down;
            Vector2 b = new Vector2(1, 1);

            Assert.That(a.DistanceSqrd(b), Is.EqualTo(5f).Within(0.1).Percent);
            Assert.That(a.Project(b), Is.EqualTo(-0.70710678119f).Within(0.1).Percent);

            Vector2 vp = a.VProject(b);
            Vector2 exp = new Vector2(-0.5f, -0.5f);
            Assert.That(vp.x, Is.EqualTo(exp.x).Within(0.1).Percent);
            Assert.That(vp.y, Is.EqualTo(exp.y).Within(0.1).Percent);

            Vector2 prod = exp.CompMultiply(exp);

            Assert.That(prod.x, Is.EqualTo(0.25f).Within(0.1).Percent);
            Assert.That(prod.y, Is.EqualTo(0.25f).Within(0.1).Percent);
        }

        [Test]
        public void CopyComponentExtensionTest()
        {
            var GO1 = new GameObject();
            var cam1 = GO1.AddComponent<Camera>();
            var ply1 = GO1.AddComponent<liPlayerCharacter>();

            cam1.nearClipPlane = 5f;
            ply1.m_runSpeed = 40f;

            var GO2 = new GameObject();

            GO2.CopyComponent(cam1);
            GO2.CopyComponent(ply1);

            var cam2 = GO2.GetComponent<Camera>();
            var ply2 = GO2.GetComponent<liPlayerCharacter>();
            
            Assert.AreEqual(cam1.nearClipPlane, cam2.nearClipPlane);
            Assert.AreEqual(ply1.m_runSpeed, ply2.m_runSpeed);
        }

        [Test]
        public void IsPrefabExtensionTest()
        {
            var pf1 = Resources.Load<GameObject>("Prefabs/Player");
            var pf2 = Resources.Load<GameObject>("Prefabs/UI/Conversation");
            var pf3 = Resources.Load<GameObject>("Prefabs/UI/Inventory");

            Assert.NotNull(pf1);
            Assert.NotNull(pf2);
            Assert.NotNull(pf3);

            Assert.IsTrue(pf1.IsPrefab());
            Assert.IsTrue(pf2.IsPrefab());
            Assert.IsTrue(pf3.IsPrefab());

            Assert.IsTrue(pf1.transform.GetChild(0).gameObject.IsPrefab());
            Assert.IsTrue(pf2.transform.GetChild(0).gameObject.IsPrefab());
            Assert.IsTrue(pf3.transform.GetChild(0).gameObject.IsPrefab());

            var obj1 = Object.Instantiate(pf1);
            var obj2 = Object.Instantiate(pf2);
            var obj3 = Object.Instantiate(pf3);

            Assert.NotNull(obj1);
            Assert.NotNull(obj2);
            Assert.NotNull(obj3);

            Assert.IsFalse(obj1.IsPrefab());
            Assert.IsFalse(obj2.IsPrefab());
            Assert.IsFalse(obj3.IsPrefab());

            Assert.IsFalse(obj1.transform.GetChild(0).gameObject.IsPrefab());
            Assert.IsFalse(obj2.transform.GetChild(0).gameObject.IsPrefab());
            Assert.IsFalse(obj3.transform.GetChild(0).gameObject.IsPrefab());
        }

        [Test]
        public void GetChildWithNameExtensionTest()
        {
            var pf1 = Resources.Load<GameObject>("Prefabs/UI/Inventory");
            
            Assert.NotNull(pf1);

            var obj1 = Object.Instantiate(pf1);

            Assert.NotNull(obj1);

            var obj2 = obj1.GetChildWithName("Panel");
            
            Assert.NotNull(obj2);

            var obj3 = obj2.GetChildWithName("Tabs");
            
            Assert.NotNull(obj3);
        }

        [Test]
        public void GetComponentOnlyInChildrenExtensionTest()
        {
            var pf1 = Resources.Load<GameObject>("Prefabs/UI/Inventory");
            
            Assert.NotNull(pf1);

            var obj1 = Object.Instantiate(pf1);

            Assert.NotNull(obj1);

            var tfrm = obj1.GetComponentOnlyInChildren<Transform>();

            Assert.NotNull(tfrm);

            Assert.That(tfrm, Is.Not.SameAs(obj1.transform));

            Assert.That(tfrm, Is.SameAs(obj1.transform.GetChild(0)));
        }
    }
}
