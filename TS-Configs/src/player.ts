import {float, int} from "./csharp-types";

/**
 * @cs-export Game.Generated.Models.Player
 */
export interface IPlayerParams
{
    speed: float;
    inventorySize: int;
}

/**
 * @cs-export Game.Generated.Query.Player:PlayerQuery
 */
function getPlayerParams(): IPlayerParams
{
    return { speed: 10.0, inventorySize: 6 };
}