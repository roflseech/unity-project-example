import {float, int} from "./csharp-types";

/**
 * @cs-export Game.Generated.Models.Inventory
 */
export interface IItemVisualData
{
    iconPath: string;
}

/**
 * @cs-export Game.Generated.Models.Inventory
 */
export interface IItemDefinition
{
    visualData: IItemVisualData;
    stackable?: boolean;
}

function createItem(visualData: IItemVisualData, stackable: boolean = true): IItemDefinition
{
    return { visualData, stackable };
}

const itemsMap = new Map<number, IItemDefinition>([
    [1, createItem({ iconPath: "Assets/GameData/UI/Icons/Items/MedKit.png[MedKit]" })],
    [2, createItem({ iconPath: "Assets/GameData/UI/Icons/Items/Rock.png[Rock]" })],
    [3, createItem({ iconPath: "Assets/GameData/UI/Icons/Items/Axe.png[MedKit]" })]
]);

/**
 * @cs-export Game.Generated.Query.Inventory:InventoryQuery
 */
function getItemById(id: int): IItemDefinition | undefined
{
    return itemsMap.get(id);
}

/**
 * @cs-export Game.Generated.Query.Inventory:InventoryQuery
 */
function getItemVisualDataById(id: int): IItemVisualData | undefined
{
    return itemsMap.get(id)?.visualData;
}

/**
 * @cs-export Game.Generated.Query.Inventory:InventoryQuery
 */
function getAllItemIds(): int[]
{
    return Array.from(itemsMap.keys());
}
