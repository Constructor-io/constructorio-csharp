using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ConstructorIO.Test
{
    [TestClass]
    public class ListItemTests
    {
        static int TestDelay = 600;

        [TestMethod]
        public void TestVerify()
        {
            var api = TestCommon.MakeAPI();

            Assert.IsTrue(api.Verify());
        }

        [TestMethod]
        [ExpectedException(typeof(ConstructorIOException))]
        public void TestCreateInvalid()
        {
            var api = TestCommon.MakeAPI();

            ListItem testItem = new ListItem(ID: "AddInvalid Test Item");

            Assert.IsTrue(api.AddOrUpdate(testItem), "Add Invalid Item");
            Task.Delay(TestDelay).Wait();
        }

        [TestMethod]
        public void TestCreateConstructor()
        {
            var api = TestCommon.MakeAPI();

            ListItem testItem = new ListItem(ID: "AddInvalid Test Item", Name: "test-item", SuggestedScore: 50,
                Description: "Sample Item", Url: "http://test.com", AutocompleteSection: "Products",
                ImageUrl: "http://test.com/test.jpg",
                Keywords: new string[]
                {
                    "keyword_a",
                    "keyword_b"
                });

            Assert.IsTrue(api.AddOrUpdate(testItem), "Add Item");
            Task.Delay(TestDelay).Wait();
            Assert.IsTrue(api.Remove(testItem), "Remove Item");
            Task.Delay(TestDelay).Wait();
        }

        [TestMethod]
        public void TestArbitraryParameter()
        {
            var api = TestCommon.MakeAPI();

            ListItem testItem = new ListItem(ID: "AddInvalid Test Item", Name: "test-item",
                Description: "Sample Item", Url: "http://test.com", AutocompleteSection: "Products",
                ImageUrl: "http://test.com/test.jpg",
                Keywords: new string[]
                {
                    "keyword_a",
                    "keyword_b"
                });

            testItem["suggested_score"] = 50;

            Assert.IsTrue(api.AddOrUpdate(testItem), "Add Item");
            Task.Delay(TestDelay).Wait();
            Assert.IsTrue(api.Remove(testItem), "Remove Item");
            Task.Delay(TestDelay).Wait();
        }

        [TestMethod]
        public void TestDictionaryParameter()
        {
            var api = TestCommon.MakeAPI();

            ListItem testItem = new ListItem(ID: "Add Test Item 3", Name: "test-item2",
                Description: "Sample Item", Url: "http://test.com", AutocompleteSection: "Products",
                ImageUrl: "http://test.com/test.jpg",
                Metadata: new Dictionary<string, string>
                {
                    { "thing1", "this" },
                    { "thing2", "that" }
                });

            testItem["suggested_score"] = 50;

            Assert.IsTrue(api.AddOrUpdate(testItem), "Add Item");
            Task.Delay(TestDelay).Wait();
//            Assert.IsTrue(api.Remove(testItem), "Remove Item");
//            Task.Delay(TestDelay).Wait();
        }

        [TestMethod]
        public void TestAddRemove()
        {
            var api = TestCommon.MakeAPI();

            ListItem testItem = new ListItem("AddRemove Test Item", ListItemAutocompleteType.Products)
            {
                ID = "AddRemoveTestItem",
                Url = "http://test.com",
                Description = "ExampleItem",
                ImageUrl = "http://test.com/test.jpg"
            };

            Assert.IsTrue(api.AddOrUpdate(testItem), "Add Item");
            Task.Delay(TestDelay).Wait();
            Assert.IsTrue(api.Remove(testItem), "Remove Item");
            Task.Delay(TestDelay).Wait();
        }

        [TestMethod]
        public void TestAddRemoveDescriptionWithQuote()
        {
            var api = TestCommon.MakeAPI();

            ListItem testItem = new ListItem("AddRemove Test Item", ListItemAutocompleteType.Products)
            {
                ID = "AddRemoveTestItem",
                Url = "http://test.com",
                Description = "ExampleItem 1\"",
                ImageUrl = "http://test.com/test.jpg"
            };

            Assert.IsTrue(api.AddOrUpdate(testItem), "Add Item");
            Task.Delay(TestDelay).Wait();
            Assert.IsTrue(api.Remove(testItem), "Remove Item");
            Task.Delay(TestDelay).Wait();
        }

        [TestMethod]
        public void TestModifyByName()
        {
            var api = TestCommon.MakeAPI();

            ListItem testItem = new ListItem("Modify Test Item", ListItemAutocompleteType.Products)
            {
                Url = "http://test.com",
                Description = "Example Item",
                ImageUrl = "http://test.com/test.jpg"
            };

            Assert.IsTrue(api.AddOrUpdate(testItem), "Add Item");
            Task.Delay(TestDelay).Wait();

            testItem.Description = "Extra details";

            Assert.IsTrue(api.Modify(testItem), "Modify Existing Item");
            Task.Delay(TestDelay).Wait();
            Assert.IsTrue(api.Remove(testItem), "Remove Item");
            Task.Delay(TestDelay).Wait();
        }

        [TestMethod]
        public void TestModifyChangeName()
        {
            var api = TestCommon.MakeAPI();

            ListItem testItem = new ListItem("Modify Test Item", ListItemAutocompleteType.Products)
            {
                Url = "http://test.com",
                Description = "Example Item",
                ImageUrl = "http://test.com/test.jpg"
            };

            Assert.IsTrue(api.AddOrUpdate(testItem), "Add Item");
            Task.Delay(TestDelay).Wait();

            testItem.Name = "Modify Test Item Rename";

            Assert.IsTrue(api.Modify(testItem), "Modify Existing Item");
            Task.Delay(TestDelay).Wait();
            Assert.IsTrue(api.Remove(testItem), "Remove Item");
            Task.Delay(TestDelay).Wait();
        }

        [TestMethod]
        public void TestModifyByID()
        {
            var api = TestCommon.MakeAPI();

            ListItem testItem = new ListItem("ModifyTestItem", ListItemAutocompleteType.Products)
            {
                ID = "ModifyTestItemID",
                Url = "http://test.com",
                Description = "Example Item",
                ImageUrl = "http://test.com/test.jpg"
            };

            Assert.IsTrue(api.AddOrUpdate(testItem), "Add Item");
            Task.Delay(TestDelay).Wait();

            testItem.Name = "ModifyTestItemNewName";

            Assert.IsTrue(api.Modify(testItem), "Modify Existing Item");
            Task.Delay(TestDelay).Wait();
            Assert.IsTrue(api.Remove(testItem), "Remove Item");
            Task.Delay(TestDelay).Wait();
        }

        [TestMethod]
        public void TestBatchRemoveByID()
        {
            var api = TestCommon.MakeAPI();

            List<ListItem> batchTestSet = new List<ListItem>();

            for (int i = 0; i < 5; i++)
            {
                batchTestSet.Add(new ListItem(ID: "BatchRemoveNum" + i)
                {
                    Name = "BatchItemNum" + i,
                    Description = "Test Item",
                    ImageUrl = "http://test.com/test.jpg",
                    Url = "http://test.com"
                });
            }

            Assert.IsTrue(api.AddOrUpdateBatch(batchTestSet, ListItemAutocompleteType.Products));
            Task.Delay(TestDelay).Wait();

            List<ListItem> removeSet = new List<ListItem>();

            for(int i=0;i<5;i++)
                removeSet.Add(new ListItem(ID:  "BatchRemoveNum" + i));

            Assert.IsTrue(api.RemoveBatch(removeSet, ListItemAutocompleteType.Products));
            Task.Delay(TestDelay).Wait();
        }

        [TestMethod]
        public void TestIndividualRemoveByID()
        {
            var api = TestCommon.MakeAPI();

            List<ListItem> batchTestSet = new List<ListItem>();

            for (int i = 0; i < 5; i++)
            {
                batchTestSet.Add(new ListItem("BatchRemoveNum" + i)
                {
                    Name = "BatchItemNum" + i,
                    Description = "Test Item",
                    ImageUrl = "http://test.com/test.jpg",
                    Url = "http://test.com"
                });
            }

            Assert.IsTrue(api.AddOrUpdateBatch(batchTestSet, ListItemAutocompleteType.Products));
            Task.Delay(TestDelay).Wait();

            List<ListItem> removeSet = new List<ListItem>();

            for (int i = 0; i < 5; i++)
            {
                Assert.IsTrue(api.Remove(new ListItem("BatchRemoveNum" + i) { AutocompleteSection = "Products" }));
                Task.Delay(TestDelay).Wait();
            }
        }


        [TestMethod]
        public void TestBatchAddIndividualRemove()
        {
            var api = TestCommon.MakeAPI();

            List<ListItem> batchTestSet = new List<ListItem>();

            for (int i = 0; i < 5; i++)
                batchTestSet.Add(new ListItem("Batch Add Ind Remove Test Item " + i, ListItemAutocompleteType.SearchSuggestions));

            Assert.IsTrue(api.AddOrUpdateBatch(batchTestSet, ListItemAutocompleteType.SearchSuggestions), "Batch Add");

            Task.Delay(500).Wait();

            foreach (var listItem in batchTestSet)
            {
                Assert.IsTrue(api.Remove(listItem), "Remove Item");
                Task.Delay(TestDelay).Wait();
            }
        }

        [TestMethod]
        public void TestBatchAddBatchRemoveWithMetaData()
        {
            var api = TestCommon.MakeAPI();

            List<ListItem> batchTestSet = new List<ListItem>();

            for (int i = 0; i < 5; i++)
            {
                ListItem testItem = new ListItem(ID: "Add Test Item " + i, Name: "BatchItemNum" + i,
                Description: "Test Item", Url: "http://test.com", AutocompleteSection: "Products",
                ImageUrl: "http://test.com/test.jpg",
                Metadata: new Dictionary<string, string>
                {
                    { "thing " + i, new Random().Next(1000).ToString() },
                    { "another thing "  + i, new Random().Next(1000).ToString() }
                });

                batchTestSet.Add(testItem);
            }

            Assert.IsTrue(api.AddOrUpdateBatch(batchTestSet, ListItemAutocompleteType.SearchSuggestions));
            Task.Delay(TestDelay).Wait();

            Assert.IsTrue(api.RemoveBatch(batchTestSet, ListItemAutocompleteType.SearchSuggestions), "Batch Remove");
            Task.Delay(TestDelay).Wait();
        }

        [TestMethod]
        public void TestBatchAddBatchRemove()
        {
            var api = TestCommon.MakeAPI();

            List<ListItem> batchTestSet = new List<ListItem>();

            for (int i = 0; i < 5; i++)
                batchTestSet.Add(new ListItem("Batch Add Batch Remove Test Item " + i, ListItemAutocompleteType.SearchSuggestions));

            Assert.IsTrue(api.AddOrUpdateBatch(batchTestSet, ListItemAutocompleteType.SearchSuggestions));
            Task.Delay(TestDelay).Wait();

            Assert.IsTrue(api.RemoveBatch(batchTestSet, ListItemAutocompleteType.SearchSuggestions), "Batch Remove");
            Task.Delay(TestDelay).Wait();
        }

        [TestMethod]
        public void TestIndividualAddBatchRemove()
        {
            var api = TestCommon.MakeAPI();

            List<ListItem> batchTestSet = new List<ListItem>();

            for (int i = 0; i < 5; i++)
                batchTestSet.Add(new ListItem("Ind Add Batch Remove Test Item " + i, ListItemAutocompleteType.SearchSuggestions));
            
            foreach (var listItem in batchTestSet)
            {
                Assert.IsTrue(api.AddOrUpdate(listItem), "Remove Item");
                Task.Delay(TestDelay).Wait();
            }

            Assert.IsTrue(api.RemoveBatch(batchTestSet, ListItemAutocompleteType.SearchSuggestions), "Batch Remove");

            Task.Delay(TestDelay).Wait();
        }
    }
}
