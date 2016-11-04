using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructorIO
{
    public class ConstructorIOAPI
    {
        private WebRequestor _webRequestor;
        private Tracker _tracker;

        public ConstructorIOAPI(string APIKey, string AutocompleteKey, string Host = "ac.cnstrc.com")
        {
            _webRequestor = new WebRequestor(APIKey, AutocompleteKey, Host);
            _tracker = new Tracker(this);
        }

        #region Methods

        /// <summary>
        /// Verify that the API key is valid.
        /// </summary>
        /// <returns>true if successful</returns>
        public async Task<bool> VerifyAsync()
        {
            var verifyRequest = new ConstructorIORequest(APIRequestType.V1_Verify, "GET");

            var verifyResponse = await Requestor.MakeRequest(verifyRequest);

            if (verifyResponse.Item2.IndexOf("successful authentication") != -1)
            {
                //TODO: beter way to do this?
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds an Item to your autocomplete.
        /// </summary>
        /// <param name="Item">The item to add</param>
        /// <returns>true if successful</returns>
        public async Task<bool> AddAsync(ListItem Item)
        {
            string requestMethod = "POST";
            var addRequest = new ConstructorIORequest(APIRequestType.V1_Item, requestMethod);

            Item.GetAsHash().ToList().ForEach((kvp) => addRequest.RequestBody.Add(kvp.Key, kvp.Value));

            var addResponse = await Requestor.MakeRequest(addRequest);
            return addResponse.Item1;
        }

        /// <summary>
        /// Adds an Item or Updates an existing item in your autocomplete.
        /// </summary>
        /// <param name="Item">The item to add or update</param>
        /// <returns>true if successful</returns>
        public async Task<bool> AddOrUpdateAsync(ListItem Item)
        {
            string requestMethod = "PUT";
            var addRequest = new ConstructorIORequest(APIRequestType.V1_Item, requestMethod);

            Item.GetAsHash().ToList().ForEach((kvp) => addRequest.RequestBody.Add(kvp.Key, kvp.Value));

            addRequest["force"] = "1";

            var addResponse = await Requestor.MakeRequest(addRequest);
            return addResponse.Item1;
        }

        /// <summary>
        /// Adds multiple items in one batch upload.
        /// </summary>
        /// <param name="Items">The items to add.</param>
        /// <param name="AutocompleteSection">The section the items should be added to.</param>
        /// <returns>true if successful</returns>
        public async Task<bool> AddBatchAsync(IEnumerable<ListItem> Items, string AutocompleteSection)
        {
            string requestMethod = "POST";
            var addBatchRequest = new ConstructorIORequest(APIRequestType.V1_BatchItems, requestMethod);

            addBatchRequest.RequestBody["items"] = Items.ToArray().Select(li => li.GetAsHash());
            addBatchRequest.RequestBody["autocomplete_section"] = AutocompleteSection;

            var addBatchResponse = await Requestor.MakeRequest(addBatchRequest);
            return addBatchResponse.Item1;
        }

        /// <summary>
        /// Adds or Updates multiple items in one batch.
        /// </summary>
        /// <param name="Items">The items to add or update.</param>
        /// <param name="AutocompleteSection">The section the items should be added to.</param>
        /// <returns>true if successful</returns>
        public async Task<bool> AddOrUpdateBatchAsync(IEnumerable<ListItem> Items, string AutocompleteSection)
        {
            string requestMethod = "PUT";
            var addBatchRequest = new ConstructorIORequest(APIRequestType.V1_BatchItems, requestMethod);

            addBatchRequest.RequestBody["items"] = Items.ToArray().Select(li => li.GetAsHash());
            addBatchRequest.RequestBody["autocomplete_section"] = AutocompleteSection;
            
            addBatchRequest["force"] = "1";

            var addBatchResponse = await Requestor.MakeRequest(addBatchRequest);
            return addBatchResponse.Item1;
        }

        /// <summary>
        /// Modifies an existing item
        /// </summary>
        /// <param name="ItemToUpdate">The item to udpate</param>
        /// <returns>true if successful</returns>
        public async Task<bool> ModifyAsync(ListItem ItemToUpdate)
        {
            var modifyRequest = new ConstructorIORequest(APIRequestType.V1_Item, "PUT");

            Util.Merge(ItemToUpdate.GetAsModifyHash(), modifyRequest.RequestBody);

            var modifyResponse = await Requestor.MakeRequest(modifyRequest);
            return modifyResponse.Item1;
        }

        /// <summary>
        /// Removes an item from your autocomplete.
        /// </summary>
        /// <param name="ItemToRemove">The item to remove.</param>
        /// <returns>true if successful</returns>
        public async Task<bool> RemoveAsync(ListItem ItemToRemove)
        {
            var removeRequest = new ConstructorIORequest(APIRequestType.V1_Item, "DELETE");

            Util.Merge(ItemToRemove.GetAsRemoveHash(), removeRequest.RequestBody);

            var removeResponse = await Requestor.MakeRequest(removeRequest);
            return removeResponse.Item1;
        }

        /// <summary>
        /// Removed multiple items from your autocomplete.
        /// </summary>
        /// <param name="ItemsToRemove">The items to remove.</param>
        /// <param name="AutocompleteSection">The autocomplete section to remove from.</param>
        /// <returns>true if successful</returns>
        public async Task<bool> RemoveBatchAsync(IEnumerable<ListItem> ItemsToRemove, string AutocompleteSection)
        {
            var removeBatchRequest = new ConstructorIORequest(APIRequestType.V1_BatchItems, "DELETE");

            removeBatchRequest.RequestBody["items"] = ItemsToRemove.ToArray().Select(item => item.GetAsRemoveHash());
            removeBatchRequest.RequestBody["autocomplete_section"] = AutocompleteSection;

            var removeBatchResponse = await Requestor.MakeRequest(removeBatchRequest);
            return removeBatchResponse.Item1;
        }

        #endregion

        #region Non Async Implementations

        //NOTE: the GetAwaiter().GetResult(); allows any thrown exceptions to not be encapsulated in
        //an aggregateException which tasks will do by default.

        /// <summary>
        /// Verify that the API key is valid.
        /// </summary>
        /// <returns>true if successful</returns>
        public bool Verify()
        {
            return VerifyAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Adds Item to your autocomplete.
        /// </summary>
        /// <param name="Item">The item to add</param>
        /// <returns>true if successful</returns>
        public bool Add(ListItem Item)
        {
            return AddAsync(Item).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Adds an Item or Updates an existing item in your autocomplete.
        /// </summary>
        /// <param name="Item">The item to add or update</param>
        /// <returns>true if successful</returns>
        public bool AddOrUpdate(ListItem Item)
        {
            return AddOrUpdateAsync(Item).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Adds multiple items in one batch upload.
        /// </summary>
        /// <param name="Items">The items to add.</param>
        /// <param name="AutocompleteSection">The section the items should be added to.</param>
        /// <returns>true if successful</returns>
        public bool AddBatch(IEnumerable<ListItem> Items, ListItemAutocompleteType AutocompleteSection)
        {
            return AddBatch(Items, StringEnum.GetStringValue(AutocompleteSection));
        }

        /// <summary>
        /// Adds multiple items in one batch upload.
        /// </summary>
        /// <param name="Items">The items to add.</param>
        /// <param name="AutocompleteSection">The section the items should be added to.</param>
        /// <returns>true if successful</returns>
        public bool AddBatch(IEnumerable<ListItem> Items, string AutocompleteSection)
        {
            return AddBatchAsync(Items, AutocompleteSection).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Adds multiple items in one batch upload.
        /// </summary>
        /// <param name="Items">The items to add.</param>
        /// <param name="AutocompleteSection">The section the items should be added to.</param>
        /// <returns>true if successful</returns>
        public async Task<bool> AddBatchAsync(IEnumerable<ListItem> Items, ListItemAutocompleteType AutocompleteSection)
        {
            return await AddBatchAsync(Items, StringEnum.GetStringValue(AutocompleteSection));
        }

        /// <summary>
        /// Adds or Updates multiple items in one batch.
        /// </summary>
        /// <param name="Items">The items to add or update.</param>
        /// <param name="AutocompleteSection">The section the items should be added to.</param>
        /// <returns>true if successful</returns>
        public bool AddOrUpdateBatch(IEnumerable<ListItem> Items, ListItemAutocompleteType AutocompleteSection)
        {
            return AddOrUpdateBatch(Items, StringEnum.GetStringValue(AutocompleteSection));
        }

        /// <summary>
        /// Adds or Updates multiple items in one batch.
        /// </summary>
        /// <param name="Items">The items to add or update.</param>
        /// <param name="AutocompleteSection">The section the items should be added to.</param>
        /// <returns>true if successful</returns>
        public bool AddOrUpdateBatch(IEnumerable<ListItem> Items, string AutocompleteSection)
        {
            return AddOrUpdateBatchAsync(Items, AutocompleteSection).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Adds or Updates multiple items in one batch.
        /// </summary>
        /// <param name="Items">The items to add or update.</param>
        /// <param name="AutocompleteSection">The section the items should be added to.</param>
        /// <returns>true if successful</returns>
        public async Task<bool> AddOrUpdateBatchAsync(IEnumerable<ListItem> Items, ListItemAutocompleteType AutocompleteSection)
        {
            return await AddOrUpdateBatchAsync(Items, StringEnum.GetStringValue(AutocompleteSection));
        }

        /// <summary>
        /// Modifies an existing item
        /// </summary>
        /// <param name="ItemToUpdate">The item to udpate</param>
        /// <returns>true if successful</returns>
        public bool Modify(ListItem ItemToModify)
        {
            return ModifyAsync(ItemToModify).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Removes an item from your autocomplete.
        /// </summary>
        /// <param name="ItemToRemove">The item to remove.</param>
        /// <returns>true if successful</returns>
        public bool Remove(ListItem Item)
        {
            return RemoveAsync(Item).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Removed multiple items from your autocomplete.
        /// </summary>
        /// <param name="ItemsToRemove">The items to remove.</param>
        /// <param name="AutocompleteSection">The autocomplete section to remove from.</param>
        /// <returns>true if successful</returns>
        public bool RemoveBatch(IEnumerable<ListItem> Items, ListItemAutocompleteType AutocompleteSection)
        {
            return RemoveBatch(Items, StringEnum.GetStringValue(AutocompleteSection));
        }

        /// <summary>
        /// Removed multiple items from your autocomplete.
        /// </summary>
        /// <param name="ItemsToRemove">The items to remove.</param>
        /// <param name="AutocompleteSection">The autocomplete section to remove from.</param>
        /// <returns>true if successful</returns>
        public bool RemoveBatch(IEnumerable<ListItem> Items, string AutocompleteSection)
        {
            return RemoveBatchAsync(Items, AutocompleteSection).GetAwaiter().GetResult();
        }

        #endregion



        internal WebRequestor Requestor
        {
            get
            {
                return _webRequestor;
            }
        }

        /// <summary>
        /// Contains the body of the last downloaded message. Useful for debugging.
        /// </summary>
        public string LastResponseBody
        {
            get
            {
                return _webRequestor.GetLastBody();
            }
        }

        /// <summary>
        /// Tracker for behavioral data.
        /// </summary>
        public Tracker Tracker
        {
            get
            {
                return _tracker;
            }
        }
    }
}