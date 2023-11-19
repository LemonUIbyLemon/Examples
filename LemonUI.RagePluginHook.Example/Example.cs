using LemonUI.Menus;
using Rage;
using Rage.Attributes;
using System;

[assembly: Plugin("LemonUI RPH Example", Description = "An example of how you can use LemonUI in RagePluginHook.", Author = "Lemon")]

namespace LemonUI.RagePluginHook.Example;

public static class Example
{
    // An object pool is a manger for almost all of LemonUI
    // You need to have only a single pool per mod
    private static readonly ObjectPool pool = new ObjectPool();

    // This is where we need to create our menus
    // You can create any amount of menus you would like to have in your mod
    // Just remember to add them to your pool in your constructor!
    // The first parameter is the title shown on top of the banner
    // The second parameter is the subtitle shown on top of a black background
    // The third parameter is the description, which is only used on submenu items
    private static readonly NativeMenu menu = new NativeMenu("LemonUI", "Welcome to LemonUI!");
    private static readonly NativeMenu submenu = new NativeMenu("LemonUI", "Oh, a submenu!", "Oh wait, this will open a submenu?");

    public static void Main()
    {
        // Now is time to create all of the items we need
        // All items take different parameters, but they usually start with the same 3 at the beginning:
        // The first parameter is the title of the item
        // The second parameter is the description of the item, shown under the list of items

        // This is a regular item, the only thing you can do is activate it
        NativeItem regularItem = new NativeItem("Regular Item", "This is a regular NativeItem, you can only activate it.");
        // This is a checkbox item, which has a checkbox that you can turn on and off
        // The third parameter sets the default state of the checkbox when you add the item, in this case false means enabled
        NativeCheckboxItem checkboxItem = new NativeCheckboxItem("Checkbox Item", "This is a NativeCheckboxItem that contains a checkbox that can be turned on and off.", true);
        // This is a dynamic list item, which is like a list item (see below) but allows you to dynamically change the items
        // For this example, we are going to use this dynamic item to create an item that can be used to increase and decrease the value of a number
        // The third parameters sets the default object shown as selected when you add the item for the first time, in this case the number 10
        NativeDynamicItem<int> dynamicItem = new NativeDynamicItem<int>("Dynamic Item", "This is a NativeDynamicItem that allows you to dynamically change the objects in the list.", 10);
        // This is a regular list item, that allows you to select a specific number of items
        // For this example
        NativeListItem<string> listItem = new NativeListItem<string>("List Item", "This is a NativeListItem that allows you to select a pre set number of items.");

        // Now that we have our items, we add them to our menu
        // All items and submenus are added by calling the function Add
        // They will be shown in the order that you called Add
        menu.Add(submenu);
        menu.Add(regularItem);
        menu.Add(checkboxItem);
        menu.Add(dynamicItem);
        menu.Add(listItem);

        // This is a very important step
        // We need to add our menus to the ObjectPool, so LemonUI knows that they exist
        // Make sure to only call this once for the menus, otherwise an exception will be raised!
        pool.Add(menu);
        pool.Add(submenu);

        // Every GTA mods needs it's own Tick event, so let's make one with Fibers
        GameFiber.StartNew(OnTick);
    }

    private static void OnTick()
    {
        // In RPH, we need to tell it to execute our game fiber infinitely
        // This will make sure that it runs forever
        while (true)
        {
            // This is the most important thing you need to add to your tick event
            // It draws the menus, does the calculations under the hood and more!
            // Pretty neat, right?
            pool.Process();

            // You might be thinking, how can we open the menu?
            // It's easy! You decide how to open it!
            // Let's see a couple of examples

            // You can check if a specific Control is pressed
            // NOTE: Controls and Keys are different things
            // You can see a full list of controls at https://docs.fivem.net/docs/game-references/controls/
            // Here, we use the Duck control, which is X on a keyboard and A/X on a controller
            if (Game.IsControlJustPressed(0, GameControl.VehicleDuck))
            {
                // You might be thinking: What's going on over here?
                // It's simple
                // Because we have more than one menu, we check if ANY menu is open
                // If any menu is open, we close them all
                // If there are no menus open, we open the main menu

                if (pool.AreAnyVisible)
                {
                    pool.HideAll();
                }
                else
                {
                    menu.Visible = true;
                }
            }

            // Remember that RPH uses Fibers, and fibers need to yield so they don't block 
            GameFiber.Yield();
        }
    }

    // You can also register a console command
    // This console command is called "Menu" and does the same as the Control press

    [ConsoleCommand("Toggles the LemonUI menu.")]
    public static void Menu()
    {
        if (pool.AreAnyVisible)
        {
            pool.HideAll();
        }
        else
        {
            menu.Visible = true;
        }
    }
}
