using NUnit.Framework;
using NUnit.Framework.Internal;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EconomyMgr : MonoBehaviour
{
    [SerializeField] TMP_Text t_money, t_catalog, t_inventory, msgbox;
    [SerializeField] TMP_Dropdown dd_catalog, dd_inventory;
    List<CatalogItem> catitems;
    List<ItemInstance> myInventoryList;
    List<string> itemNames = new List<string>();
    ItemInstance selectedItem;

    ///////////////////////////////// HEIN LINN HTET 243962J  SHOP CATALOG /////////////////////////////////
    public void GetCatalog()
    {
        var catReq = new GetCatalogItemsRequest { CatalogVersion = "FruitCatalog" };
        PlayFabClientAPI.GetCatalogItems(catReq,
            carRes =>
            {
                catitems = carRes.Catalog;
                t_catalog.text = "Catalog Items\n";
                t_catalog.text += "-------------\n";
                List<string> itemNames = new List<string>();
                foreach (CatalogItem i in catitems)
                {
                    t_catalog.text += (i.DisplayName + ":" + i.VirtualCurrencyPrices["CN"] + "\n");
                    itemNames.Add(i.DisplayName + "[" + i.VirtualCurrencyPrices["CN"]+"]");
                }
                dd_catalog.ClearOptions();
                dd_catalog.AddOptions(itemNames);
                dd_catalog.Show();
            },OnError);
    }

    void OnError(PlayFabError e)
    {
        msgbox.text = "Error:" + e.GenerateErrorReport();
    }

    ///////////////////////////////// HEIN LINN HTET 243962J  SHOP BUY /////////////////////////////////
    public void BuyDropDownItem()
    {
        int index = dd_catalog.value;
        CatalogItem selectedItem = catitems[index];
        var buyReq = new PurchaseItemRequest
        {
            CatalogVersion = "FruitCatalog",
            ItemId = selectedItem.ItemId,
            VirtualCurrency = "CN",
            Price = (int)selectedItem.VirtualCurrencyPrices["CN"]
        };
        PlayFabClientAPI.PurchaseItem(buyReq,
            result =>
            {
                t_money.text = "Bought " + selectedItem.DisplayName;
                GetPlayerInventory();
                GetVirtualCurrency();
            },OnError);
    }
    ///////////////////////////////// HEIN LINN HTET 243962J  SHOP INVENTORY /////////////////////////////////
    public void GetPlayerInventory()
    {
        var UserInvReq = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(UserInvReq,
        result =>
        {
            myInventoryList = result.Inventory;
            List<string> itemNames = new List<string>();
            t_inventory.text = "My Inventory\n";
            t_inventory.text += "-------------\n";
            foreach (ItemInstance ii in myInventoryList)
            {
                t_inventory.text += (ii.DisplayName + "\n");
                itemNames.Add(ii.DisplayName);
            }
            dd_inventory.ClearOptions();
            dd_inventory.AddOptions(itemNames);
            dd_inventory.Show();
        }, OnError);
    }
    public void OnUseItem()
    {
        int idx2BeConsumed = dd_inventory.value;
        selectedItem = myInventoryList[idx2BeConsumed];
        ConsumePlayerItem(selectedItem.ItemInstanceId);
    }
    public void ConsumePlayerItem(string itemInstanceId)
    {
        var request = new ConsumeItemRequest
        {
            ItemInstanceId = itemInstanceId,
            ConsumeCount = 1
        };
        PlayFabClientAPI.ConsumeItem(request, OnConsumeSuccess, OnError);
    }
    void OnConsumeSuccess(ConsumeItemResult result)
    {
        msgbox.text = $"{selectedItem.DisplayName} consumed.";
        GetPlayerInventory();
    }
    ///////////////////////////////// HEIN LINN HTET 243962J  SHOP MONEY /////////////////////////////////
    public void OnGivePlayerMoney()
    {
        var req = new AddUserVirtualCurrencyRequest
        {
            VirtualCurrency = "CN",
            Amount = 50
        };
        PlayFabClientAPI.AddUserVirtualCurrency(req, OnGivePlayerMoneySucc, OnError);
    }
    void OnGivePlayerMoneySucc(ModifyUserVirtualCurrencyResult res)
    {
        msgbox.text = "Gave " + res.BalanceChange + " coins. Balance:" + res.Balance;
        GetVirtualCurrency();
    }
    public void GetVirtualCurrency()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            r =>
            {
                int coins = r.VirtualCurrency["CN"];
                t_money.text = "Coins:" + coins;

            }, OnError);
    }
    private void Start()
    {
        GetVirtualCurrency();
        GetCatalog();
        GetPlayerInventory();
    }
}
