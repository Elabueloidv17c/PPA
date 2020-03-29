using System.Collections;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;

namespace Tests
{
    public class InventoryTestScript
    {
        [SetUp]
        public void Setup()
        {
            GameObject GO2 = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Game Manager"));
        }

        [TearDown]
        public void Teardown()
        {
            Object.Destroy(liGameManager.instance.gameObject);

            Object.Destroy(liInventory.instance.gameObject);
            Object.Destroy(liDialogManager.instance.gameObject);
            Object.Destroy(EventSystem.current.gameObject);
        }

        [UnityTest]
        public IEnumerator InventoryTest()
        {
            Assert.NotNull(liGameManager.instance);

            Assert.NotNull(liInventory.instance);
            Assert.NotNull(liDialogManager.instance);
            Assert.NotNull(EventSystem.current);

            liInventory.instance.AddItem(0);
            liInventory.instance.AddItem(0);
            liInventory.instance.AddItem(0);
            liInventory.instance.AddItem(0);
            liInventory.instance.AddItem(0);
            
            Assert.That(liInventory.s_currentItems[0].count, Is.EqualTo(5));

            var eventManager = 
                liGameManager.instance.GetComponent<liEventManager>();

            Assert.NotNull(eventManager);
            
            eventManager.InspectorSaveFile();
            
            yield return new WaitForSeconds(2f);
            
            eventManager.InspectorRestoreFile();
            
            yield return new WaitForSeconds(2f);
            
            Assert.That(liInventory.s_currentItems[0].count, Is.EqualTo(5));

            yield return null;
        }
    }
}
