import {int} from "./csharp-types";

/**
 * @cs-export Game.Generated.Models.Inventory
 */
interface IInventoryParams
{
    stackSize : int;
}

/**
 * @cs-export Game.Generated.Query.Inventory:InventoryQuery
 */
function getInventoryParams(): IInventoryParams
{
    return {
        stackSize: 9
    };
}