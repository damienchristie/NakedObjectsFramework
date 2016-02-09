﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Linq;

namespace NakedObjects.Web.UnitTests.Selenium
{
    public abstract class CiceroTestRoot : AWTest
    {
        public virtual void Action()
        {
            //Test from home menu
            CiceroUrl("home?menu1=ProductRepository");
            WaitForOutput("Products menu");
            //First, without paramsd
            EnterCommand("Action");
            WaitForOutputStarting("Actions:\r\n"+
                "Find Product By Name\r\n" +
                "Find Product By Number\r\n");

            //Filtered list
            EnterCommand("act cateGory  ");
            WaitForOutput("Matching actions:\r\nList Products By Sub Category\r\nFind Products By Category");
            //No match
            EnterCommand("act foo  ");
            WaitForOutput("foo does not match any actions");
            //single match
            EnterCommand("act rand");
            WaitForOutput("Products menu\r\nAction dialog: Random Product");
            //Test from object context
            CiceroUrl("object?object1=AdventureWorksModel.Product-358");
            WaitForOutput("Product: HL Grip Tape");

            EnterCommand("Action");
            WaitForOutput("Actions:\r\nAdd Or Change Photo\r\nBest Special Offer\r\nAssociate Special Offer With Product\r\nOpen Purchase Orders For Product\r\nCreate New Work Order\r\nWork Orders");
            EnterCommand("act ord");
            WaitForOutput("Matching actions:\r\nOpen Purchase Orders For Product\r\nCreate New Work Order\r\nWork Orders");
            EnterCommand("act foo  ");
            WaitForOutput("foo does not match any actions");
            EnterCommand("ac best");
            WaitForOutput("Product: HL Grip Tape\r\nAction dialog: Best Special Offer\r\nQuantity: empty");
            //Not available in current context
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("ac");
            WaitForOutput("The command: action is not available in the current context");
            //multi clause search
            CiceroUrl("home?menu1=CustomerRepository");
            WaitForOutput("Customers menu");
            EnterCommand("ac name find by");
            WaitForOutput("Matching actions:\r\nStores - Find Store By Name\r\nIndividuals - Find Individual Customer By Name");
            //Matches includes sub-menu
            EnterCommand("ac stores");
            WaitForOutput("Matching actions:\r\nStores - Find Store By Name\r\nStores - Create New Store Customer\r\nStores - Random Store");
            EnterCommand("ac ores d");
            WaitForOutput("Matching actions:\r\nStores - Find Store By Name\r\nStores - Random Store");
            EnterCommand("ac ores ran");
            WaitForOutput("Customers menu\r\nAction dialog: Random Store");
            CiceroUrl("home?menu1=CustomerRepository");
            WaitForOutput("Customers menu");
            EnterCommand("ac ran individuals"); //Order doesn't matter
            WaitForOutput("Customers menu\r\nAction dialog: Random Individual");

            //object with no actions
            CiceroUrl("object?object1=AdventureWorksModel.ProductInventory-442-6");
            WaitForOutput("Product Inventory: 524 in Miscellaneous Storage - G");
            EnterCommand("ac");
            WaitForOutput("No actions available");

            //To many args
            EnterCommand("ac name find by, x, y");
            WaitForOutput("Too many arguments provided");

            //Exact match takes precedence over partial match
            CiceroUrl("home?menu1=WorkOrderRepository");
            WaitForOutput("Work Orders menu");
            EnterCommand("ac Create New Work Order"); //which would also match Create New Work Order2
            WaitForOutput("Work Orders menu\r\nAction dialog: Create New Work Order\r\nProduct: empty");

            //Invoking action (no args) on an object with only one action goes straight to dialog
            CiceroUrl("object?object1=AdventureWorksModel.SpecialOffer-1");
            WaitForOutput("Special Offer: No Discount");
            EnterCommand("ac");
            WaitForOutput("Special Offer: No Discount\r\nAction dialog: Associate Special Offer With Product\r\nProduct: empty");

            //Disabled action - listed as such
            CiceroUrl("object?object1=AdventureWorksModel.Vendor-1644");
            WaitForOutput("Vendor: International Sport Assoc.");
            EnterCommand("ac");
            WaitForOutputContaining("Check Credit (disabled: Not yet implemented)");

            //Disabled action -  cannot open dialog
            EnterCommand("ac check credit");
            WaitForOutput("Action: Check Credit is disabled. Not yet implemented");

            //Question mark to get details
            CiceroUrl("home?menu1=PurchaseOrderRepository");
            WaitForOutput("Purchase Orders menu");
            EnterCommand("ac random,?");
            WaitForOutput("Description for action: Random Purchase Order\r\n"+
                    "For demonstration purposes only");

            //Invalid second param
            EnterCommand("ac random,x");
            WaitForOutput("Second argument may only be a question mark -  to get action details");
        }
        public virtual void BackAndForward() //Tested together for simplicity
        {
            CiceroUrl("home");
            EnterCommand("menu cus");
            WaitForOutput("Customers menu");
            EnterCommand("action random store");
            WaitForOutput("Customers menu\r\nAction dialog: Random Store");
            EnterCommand("back");
            WaitForOutput("Customers menu");
            EnterCommand("Ba");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("forward");
            WaitForOutput("Customers menu");
            EnterCommand("fO  ");
            WaitForOutput("Customers menu\r\nAction dialog: Random Store");
            //Can't go forward beyond most recent
            EnterCommand("forward");
            WaitForOutput("Customers menu\r\nAction dialog: Random Store");

            //No arguments
            EnterCommand("back x");
            WaitForOutput("Too many arguments provided");
            EnterCommand("forward y");
            WaitForOutput("Too many arguments provided");
        }
        public virtual void Cancel()
        {
            //Menu dialog
            CiceroUrl("home?menu1=ProductRepository&dialog1=FindProductByName");
            WaitForOutputStarting("Products menu\r\nAction dialog: Find Product By Name");
            EnterCommand("cancel");
            WaitForOutput("Products menu");
            //Test on a zero param action
            CiceroUrl("home?menu1=ProductRepository&dialog1=RandomProduct");
            WaitForOutput("Products menu\r\nAction dialog: Random Product");
            EnterCommand("cancel");
            WaitForOutput("Products menu");
            //Try with argument
            CiceroUrl("home?menu1=EmployeeRepository&dialog1=RandomEmployee");
            WaitForOutput("Employees menu\r\nAction dialog: Random Employee");
            EnterCommand("cancel x");
            WaitForOutput("Too many arguments provided");

            //Object dialog
            CiceroUrl("object?object1=AdventureWorksModel.Product-358&dialog1=BestSpecialOffer");
            WaitForOutputStarting("Product: HL Grip Tape\r\nAction dialog: Best Special Offer");
            EnterCommand("cancel");
            WaitForOutput("Product: HL Grip Tape");
            //Zero param
            CiceroUrl("object?object1=AdventureWorksModel.Customer-29688&dialog1=LastOrder");
            WaitForOutputStarting("Customer: Handy Bike Services, AW00029688\r\nAction dialog: Last Order");
            EnterCommand("Ca");
            WaitForOutput("Customer: Handy Bike Services, AW00029688");

            //Cancel of Edits
            CiceroUrl("object?object1=AdventureWorksModel.Product-358&edit1=true");
            WaitForOutput("Editing Product: HL Grip Tape");
            EnterCommand("cancel");
            WaitForOutput("Product: HL Grip Tape");

            //Invalid contexts
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("cancel");
            WaitForOutput("The command: cancel is not available in the current context");
            CiceroUrl("home?menu1=ProductRepository");
            WaitForOutput("Products menu");
            EnterCommand("cancel");
            WaitForOutput("The command: cancel is not available in the current context");
            CiceroUrl("object?object1=AdventureWorksModel.Customer-29688");
            WaitForOutput("Customer: Handy Bike Services, AW00029688");
            EnterCommand("cancel");
            WaitForOutput("The command: cancel is not available in the current context");
        }
        public virtual void Clipboard()
        {
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");


            CiceroUrl("object?object1=AdventureWorksModel.Person-12941");
            WaitForOutput("Person: Dakota Wood");
            EnterCommand("clipboard cop");
            WaitForOutput("Clipboard contains: Person: Dakota Wood");
            CiceroUrl("object?object1=AdventureWorksModel.Person-12942");
            WaitForOutput("Person: Jaclyn Liang");
            //Check clipboard still unmodified
            EnterCommand("clipboard sh");
            WaitForOutput("Clipboard contains: Person: Dakota Wood");
            //Now change it
            EnterCommand("clipboard cop");
            WaitForOutput("Clipboard contains: Person: Jaclyn Liang");
            //Discard
            EnterCommand("clipboard d");
            WaitForOutput("Clipboard is empty");

            //Invalid argument
            EnterCommand("clipboard copyx");
            WaitForOutput("Clipboard command may only be followed by copy, show, go, or discard");

            //No argument
            EnterCommand("clipboard");
            WaitForOutput("No arguments provided");

            //Too many arguments
            EnterCommand("clipboard copy, show");
            WaitForOutput("Too many arguments provided");

            //Attempt to copy from home
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("clipboard c");
            WaitForOutput("Clipboard copy may only be used in the context of viewing an object");
            //Attempt to copy from list
            CiceroUrl("list?menu1=SpecialOfferRepository&action1=CurrentSpecialOffers");
            WaitForOutputStarting("Result from Current Special Offers:\r\n16 items");
            EnterCommand("clipboard c");
            WaitForOutput("Clipboard copy may only be used in the context of viewing an object");
        }
        public virtual void Edit()
        {
            CiceroUrl("home");
            CiceroUrl("object?object1=AdventureWorksModel.Customer-29688");
            WaitForOutput("Customer: Handy Bike Services, AW00029688");
            EnterCommand("edit");
            WaitForOutput("Editing Customer: Handy Bike Services, AW00029688");
            //No arguments
            CiceroUrl("object?object1=AdventureWorksModel.Customer-29688");
            WaitForOutput("Customer: Handy Bike Services, AW00029688");
            EnterCommand("edit x");
            WaitForOutput("Too many arguments provided");
            //Invalid contexts
            CiceroUrl("object?object1=AdventureWorksModel.Product-358&edit1=true");
            WaitForOutput("Editing Product: HL Grip Tape");
            EnterCommand("edit");
            WaitForOutput("The command: edit is not available in the current context");
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("edit");
            WaitForOutput("The command: edit is not available in the current context");
        }
        public virtual void Enter()
        {
            //Entering fields (into dialogs)
            CiceroUrl("home?menu1=CustomerRepository&dialog1=FindIndividualCustomerByName&field1_firstName=%2522%2522&field1_lastName=%2522%2522");
            WaitForOutput("Customers menu\r\nAction dialog: Find Individual Customer By Name\r\nFirst Name: empty\r\nLast Name: empty");
            EnterCommand("enter first, Arthur G");
            WaitForOutput("Customers menu\r\nAction dialog: Find Individual Customer By Name\r\nFirst Name: Arthur G\r\nLast Name: empty");
            EnterCommand("enter last, Fenton-Jones III");
            WaitForOutput("Customers menu\r\nAction dialog: Find Individual Customer By Name\r\nFirst Name: Arthur G\r\nLast Name: Fenton-Jones III");

            //Different types (bool, date, number)
            CiceroUrl("object?object1=AdventureWorksModel.Product-897&actions1=open&dialog1=BestSpecialOffer&field1_quantity=%2522%2522");
            WaitForOutputContaining("Action dialog: Best Special Offer");
            EnterCommand("enter quantity,25");
            WaitForOutputContaining("Quantity: 25");
            CiceroUrl("home?menu1=PurchaseOrderRepository&dialog1=ListPurchaseOrders&field1_vendor=null&field1_fromDate=%2522%2522&field1_toDate=%2522%2522");
            WaitForOutputContaining("Action dialog: List Purchase Orders");
            EnterCommand("enter from, 1 Jan 2016");
            WaitForOutputContaining("From Date: 1 Jan 2016");
            CiceroUrl("object?object1=AdventureWorksModel.Customer-140&actions1=open&dialog1=CreateNewOrder&field1_copyHeaderFromLastOrder=false");
            WaitForOutputContaining("Action dialog: Create New Order");
            EnterCommand("enter copy,false");
            WaitForOutputContaining("Copy Header From Last Order: false");
            EnterCommand("enter copy,true");
            WaitForOutputContaining("Copy Header From Last Order: true");

            //Todo: test selections
            CiceroUrl("home?menu1=ProductRepository&dialog1=ListProductsBySubCategory");
            WaitForOutput("Products menu\r\nAction dialog: List Products By Sub Category");
            EnterCommand("enter cat, hand");
            WaitForOutput("Products menu\r\nAction dialog: List Products By Sub Category\r\nSub Category: Handlebars");
            EnterCommand("enter cat, xx");
            WaitForOutput("None of the choices matches xx");
            EnterCommand("enter cat, frame");
            WaitForOutput("Multiple matches:\r\nMountain Frames\r\nRoad Frames\r\nTouring Frames");

            //Then property entries
            CiceroUrl("object?object1=AdventureWorksModel.Product-871&edit1=true");
            WaitForOutput("Editing Product: Mountain Bottle Cage");
            EnterCommand("prop price");
            WaitForOutput("List Price: 9.99");
            EnterCommand("enter list price, 10.50");
            WaitForOutput("Editing Product: Mountain Bottle Cage\r\nModified properties:\r\nList Price: 10.50");

            //Multiple matches
            CiceroUrl("object?object1=AdventureWorksModel.WorkOrder-33787&edit1=true");
            WaitForOutputStarting("Editing Work Order:");
            EnterCommand("enter date,1 Jan 2015");
            WaitForOutput("date matches multiple fields:\r\n"+
                "Start Date\r\n" +
                "End Date\r\n"+
                "Due Date\r\n"+
                "Modified Date");

            //No matches
            EnterCommand("enter strt date,1 Jan 2015");
            WaitForOutput("strt date does not match any properties");

            //No entry value provided
            EnterCommand("enter start date");
            WaitForOutput("Too few arguments provided");

            //Can enter an empty value
            EnterCommand("enter end date,");
            WaitForOutputContaining("End Date: empty");

            //Invalid context
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("enter");
            WaitForOutput("The command: enter is not available in the current context");

            //TODO: Entering fields on an editable view model

            //Finish somewhere other than home!
            EnterCommand("menu products");
            WaitForOutput("Products menu");
        }
        public virtual void Gemini()
        {
            //home
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("gemini");
            WaitForView(Pane.Single, PaneType.Home);

            CiceroUrl("object?object1=AdventureWorksModel.Product-968");
            WaitForOutput("Product: Touring-1000 Blue, 54");
            EnterCommand("gemini");
            WaitForView(Pane.Single, PaneType.Object, "Touring-1000 Blue, 54");

            //No arguments
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("ge x");
            WaitForOutput("Too many arguments provided");
        }
        public virtual void Goto()
        {
            CiceroUrl("object?object1=AdventureWorksModel.Customer-577");
            WaitForOutput("Customer: Synthetic Materials Manufacturing, AW00000577");
            //Full match
            EnterCommand("go Details");
            WaitForOutput("Store: Synthetic Materials Manufacturing");
            //Partial match (on Sales Person)
            EnterCommand("go pers");
            WaitForOutput("Sales Person: Pamela Ansman-Wolfe");
            //No match
            EnterCommand("go x");
            WaitForOutput("x does not match any reference fields or collections");
            //Matches only value fields
            EnterCommand("go bonus");
            WaitForOutput("bonus does not match any reference fields or collections");

            //Multiple matches
            EnterCommand("go details");
            WaitForOutput("Multiple matches for details:\r\nEmployee Details\r\nPerson Details");
            //Multiple clause match
            EnterCommand("go details pers"); //Person Details
            WaitForOutput("Person: Pamela Ansman-Wolfe");
            //Wrong no of args
            EnterCommand("go");
            WaitForOutput("No arguments provided");
            EnterCommand("go x,y");
            WaitForOutput("Too many arguments provided");
            //Now try for a list context
            CiceroUrl("list?menu1=SpecialOfferRepository&dialog1=CurrentSpecialOffers&action1=CurrentSpecialOffers&page1=1&pageSize1=20&selected1=0");
            WaitForOutput("Result from Current Special Offers:\r\n16 items");
            EnterCommand("go 1");
            WaitForOutput("Special Offer: No Discount");
            CiceroUrl("list?menu1=SpecialOfferRepository&dialog1=CurrentSpecialOffers&action1=CurrentSpecialOffers&page1=1&pageSize1=20&selected1=0");
            WaitForOutput("Result from Current Special Offers:\r\n16 items");
            EnterCommand("go 16");
            WaitForOutput("Special Offer: Mountain-500 Silver Clearance Sale");
            //Try out of range
            CiceroUrl("list?menu1=SpecialOfferRepository&dialog1=CurrentSpecialOffers&action1=CurrentSpecialOffers&page1=1&pageSize1=20&selected1=0");
            WaitForOutput("Result from Current Special Offers:\r\n16 items");
            EnterCommand("go 0");
            WaitForOutput("0 is out of range for displayed items");
            EnterCommand("go 17");
            WaitForOutput("17 is out of range for displayed items");
            EnterCommand("go x");
            WaitForOutput("x is not a valid number");
            EnterCommand("go 1x"); //Because of behaviour of tryParse
            WaitForOutput("Special Offer: No Discount");
            //Wrong context
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("go x");
            WaitForOutput("The command: goto is not available in the current context");
            CiceroUrl("home?menu1=ProductRepository&dialog1=RandomProduct");
            WaitForOutput("Products menu\r\nAction dialog: Random Product");
            EnterCommand("go x");
            WaitForOutput("The command: goto is not available in the current context");

            //Goto a collection within an object
            CiceroUrl("object?object1=AdventureWorksModel.SalesOrderHeader-60485");
            WaitForOutput("Sales Order Header: SO60485");

            //Simple case
            EnterCommand("goto details");
            WaitForOutput("Collection: Details on Sales Order Header: SO60485\r\n3 items");

            //Multiple matches
            CiceroUrl("object?object1=AdventureWorksModel.Product-901");
            WaitForOutput("Product: LL Touring Frame - Yellow, 54");
            EnterCommand("goto pr");
            WaitForOutput("Multiple matches for pr:\r\nProduct Model\r\nProduct Category\r\nProduct Subcategory\r\nProduct Inventory\r\nProduct Reviews");

            //Multi-clause match
            EnterCommand("goto v ory");
            WaitForOutput("Collection: Product Inventory on Product: LL Touring Frame - Yellow, 54\r\nempty");

            //No matches
            CiceroUrl("object?object1=AdventureWorksModel.SalesOrderHeader-60485");
            WaitForOutput("Sales Order Header: SO60485");
            EnterCommand("go x son");
            WaitForOutput("x son does not match any reference fields or collections");

            //Too many arguments
            EnterCommand("go de,tails");
            WaitForOutput("Too many arguments provided");

        }
        public virtual void Help()
        {
            //Help from home
            CiceroUrl("home");
            EnterCommand("help");
            WaitForOutput("Commands available in current context:\r\nback\r\nclipboard\r\nforward\r\ngemini\r\nhelp\r\nmenu\r\nwhere");
            //Now try an object context
            CiceroUrl("object?object1=AdventureWorksModel.Product-943");
            WaitForOutput("Product: LL Mountain Frame - Black, 40");
            //First with no params
            EnterCommand("help");
            WaitForOutput("Commands available in current context:\r\naction\r\nback\r\nclipboard\r\nedit\r\nforward\r\ngemini\r\ngoto\r\nhelp\r\nmenu\r\nproperty\r\nreload\r\nwhere");
            //Now with params
            EnterCommand("help me");
            WaitForOutput("menu command:\r\nOpen a named main menu, from any context. Menu takes one optional argument: the name, or partial name, of the menu. If the partial name matches more than one menu, a list of matches is returned but no menu is opened; if no argument is provided a list of all the menus is returned.");
            EnterCommand("help menux");
            WaitForOutput("No such command: menux");
            EnterCommand("help menu back");
            WaitForOutput("No such command: menu back");
            EnterCommand("help menu, back");
            WaitForOutput("Too many arguments provided");
            //List context
            CiceroUrl("list?menu1=SpecialOfferRepository&action1=CurrentSpecialOffers&page1=1&pageSize1=20&selected1=0");
            WaitForOutputStarting("Result from Current Special Offers:");
            EnterCommand("help");
            WaitForOutput("Commands available in current context:\r\naction\r\nback\r\nclipboard\r\nforward\r\ngemini\r\ngoto\r\nhelp\r\nmenu\r\npage\r\nreload\r\nselection\r\nshow\r\nwhere");
        }
        public virtual void Menu()
        {   //No argument
            CiceroUrl("home");
            EnterCommand("Menu");
            WaitForOutput("Menus:\r\nCustomers\r\nOrders\r\nProducts\r\nEmployees\r\nSales\r\nSpecial Offers\r\nContacts\r\nVendors\r\nPurchase Orders\r\nWork Orders");
            //Now with arguments
            EnterCommand("Menu Customers");
            WaitForOutput("Customers menu");
            EnterCommand("Menu pro");
            WaitForOutput("Products menu");
            EnterCommand("Menu off");
            WaitForOutput("Special Offers menu");
            EnterCommand("Menu ord");
            WaitForOutput("Matching menus:\r\nOrders\r\nPurchase Orders\r\nWork Orders");
            EnterCommand("Menu foo");
            WaitForOutput("foo does not match any menu");
            EnterCommand("Menu cust prod");
            WaitForOutput("cust prod does not match any menu");
            EnterCommand("Menu cus, ord");
            WaitForOutput("Too many arguments provided");
            //Invoked in another context
            CiceroUrl("object?object1=AdventureWorksModel.Product-943");
            WaitForOutput("Product: LL Mountain Frame - Black, 40");
            EnterCommand("Menu");
            WaitForOutput("Menus:\r\nCustomers\r\nOrders\r\nProducts\r\nEmployees\r\nSales\r\nSpecial Offers\r\nContacts\r\nVendors\r\nPurchase Orders\r\nWork Orders");

            //Test for exact match taking precedence over partial matches
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("menu orders"); //which would match 3, but one exactly
            WaitForOutput("Orders menu");

            //Invoking menu from a non-home context, clears state
            CiceroUrl("list?menu1=SpecialOfferRepository&action1=CurrentSpecialOffers");
            WaitForOutputStarting("Result from Current Special Offers:");
            EnterCommand("menu cus");
            WaitForOutput("Customers menu");
        }
        public virtual void OK()
        {
            CiceroUrl("home");
            //Open a zero-param action on main menu
            CiceroUrl("home?menu1=ProductRepository&dialog1=RandomProduct");
            WaitForOutput("Products menu\r\nAction dialog: Random Product");
            EnterCommand("ok");
            WaitForOutputStarting("Product: ");
            // No arguments
            CiceroUrl("home?menu1=ProductRepository&dialog1=RandomProduct");
            WaitForOutput("Products menu\r\nAction dialog: Random Product");
            EnterCommand("ok x");
            WaitForOutput("Too many arguments provided");

            //Object action
            CiceroUrl("object?object1=AdventureWorksModel.Customer-29688&dialog1=LastOrder");
            WaitForOutput("Customer: Handy Bike Services, AW00029688\r\nAction dialog: Last Order");
            EnterCommand("ok");
            WaitForOutput("Sales Order Header: SO69562");

            //Invalid contexts
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("ok");
            WaitForOutput("The command: ok is not available in the current context");
            CiceroUrl("home?menu1=ProductRepository");
            WaitForOutput("Products menu");
            EnterCommand("ok");
            WaitForOutput("The command: ok is not available in the current context");

            //Menu action that returns a List
            CiceroUrl("home?menu1=OrderRepository&dialog1=HighestValueOrders");
            WaitForOutput("Orders menu\r\nAction dialog: Highest Value Orders");
            EnterCommand("ok");
            WaitForOutputStarting("Result from Highest Value Orders:\r\nPage 1 of ");

            //Menu action with params
            CiceroUrl("home?menu1=CustomerRepository&dialog1=FindIndividualCustomerByName&field1_firstName=%2522a%2522&field1_lastName=%2522b%2522");
            WaitForOutput("Customers menu\r\nAction dialog: Find Individual Customer By Name\r\nFirst Name: a\r\nLast Name: b");
            EnterCommand("ok");
            WaitForOutputStarting("Result from Find Individual Customer By Name:\r\nPage 1 of 8 containing 20 of");

            //Menu action with missing mandatory params
            CiceroUrl("home?menu1=CustomerRepository&dialog1=FindIndividualCustomerByName&field1_firstName=%2522a%2522&field1_lastName=%2522%2522");
            WaitForOutput("Customers menu\r\nAction dialog: Find Individual Customer By Name\r\nFirst Name: a\r\nLast Name: empty");
            EnterCommand("ok");
            WaitForOutput("Please complete or correct these fields:\r\nLast Name: required");

            //Menu action with invalid entry
            CiceroUrl("home?menu1=CustomerRepository&dialog1=FindCustomerByAccountNumber&field1_accountNumber=%252212345%2522");
            WaitForOutput("Customers menu\r\nAction dialog: Find Customer By Account Number\r\nAccount Number: 12345");
            EnterCommand("ok");
            WaitForOutput("Please complete or correct these fields:\r\nAccount Number: 12345 Account number must start with AW");

            //Menu action with select param
            CiceroUrl("home?menu1=ProductRepository&dialog1=ListProductsBySubCategory&field1_subCategory=%257B%2522href%2522%253A%2522http%253A%252F%252Flocalhost%253A61546%252Fobjects%252FAdventureWorksModel.ProductSubcategory%252F10%2522%252C%2522title%2522%253A%2522Forks%2522%257D");
            WaitForOutput("Products menu\r\nAction dialog: List Products By Sub Category\r\nSub Category: Forks");
            EnterCommand("ok");
            WaitForOutputStarting("Result from List Products By Sub Category:\r\n3 items");

            //Action resulting in an error
            CiceroUrl("home?menu1=CustomerRepository&dialog1=ThrowDomainException");
            WaitForOutput("Customers menu\r\nAction dialog: Throw Domain Exception");
            EnterCommand("ok");
            WaitForOutput("Sorry, an application error has occurred. Foo");

            //Co-validation error
            CiceroUrl("object?object1=AdventureWorksModel.Vendor-1668&actions1=open&dialog1=ListPurchaseOrders&field1_fromDate=%25222016-01-06T00%253A00%253A00.000Z%2522&field1_toDate=%25222016-01-05T00%253A00%253A00.000Z%2522");
            WaitForOutputStarting("Vendor: Touring Equipment Center\r\n"+
                "Action dialog: List Purchase Orders");
            EnterCommand("ok");
            WaitForOutput("To Date cannot be before From Date");
        }
        public virtual void Page()
        {
            CiceroUrl("list?menu1=OrderRepository&action1=HighestValueOrders&page1=1&pageSize1=20&selected1=0");
            WaitForOutputStarting("Result from Highest Value Orders:\r\nPage 1 of 1574");
            EnterCommand("page 0");
            WaitForOutput("Specified page number must be between 1 and 1574");
            EnterCommand("page 2");
            WaitForOutputStarting("Result from Highest Value Orders:\r\nPage 2 of 1574");
            EnterCommand("page 1574");
            WaitForOutputStarting("Result from Highest Value Orders:\r\nPage 1574 of 1574");
            EnterCommand("page 1575");
            WaitForOutput("Specified page number must be between 1 and 1574");
            EnterCommand("page f");
            WaitForOutputStarting("Result from Highest Value Orders:\r\nPage 1 of 1574");
            EnterCommand("page p");
            WaitForOutput("List is already showing the first page");
            EnterCommand("pa last");
            WaitForOutputStarting("Result from Highest Value Orders:\r\nPage 1574 of 1574");
            EnterCommand("pa nex");
            WaitForOutput("List is already showing the last page");
            EnterCommand("page p");
            WaitForOutputStarting("Result from Highest Value Orders:\r\nPage 1573 of 1574");
            EnterCommand("pa ne");
            WaitForOutputStarting("Result from Highest Value Orders:\r\nPage 1574 of 1574");

            //Invalid argument
            EnterCommand("page furst");
            WaitForOutput("The argument must match: first, previous, next, last, or a single number");

            //No argument
            EnterCommand("page");
            WaitForOutput("No arguments provided");

            //Invalid context
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("page 1");
            WaitForOutput("The command: page is not available in the current context");
            CiceroUrl("object?object1=AdventureWorksModel.SalesOrderHeader-51131");
            WaitForOutput("Sales Order Header: SO51131");
            EnterCommand("page 1");
            WaitForOutput("The command: page is not available in the current context");
        }
        public virtual void Property()
        {
            CiceroUrl("object?object1=AdventureWorksModel.Product-758");
            WaitForOutput("Product: Road-450 Red, 52");
            EnterCommand("prop num");
            WaitForOutput("Product Number: BK-R68R-52");
            EnterCommand("pr product");
            WaitForOutputStarting("Product Number: BK-R68R-52");
            var actual = WaitForCss(".output").Text;
            //Note that spacing of Road-450 and  Line: R is different to how it appears on screen!
            var expected = "Product Number: BK-R68R-52\r\nProduct Model: Road-450\r\nProduct Category: Bikes\r\nProduct Subcategory: Road Bikes\r\nProduct Line: R \r\nProduct Inventory (collection): 2 items\r\nProduct Reviews (collection): empty";
            Assert.AreEqual(expected, actual);

            //No argument
            EnterCommand("pr ");
            WaitForOutputStarting("Name: Road-450 Red, 52\r\nProduct Number: BK-R68R-52\r\nColor: Red\r\nPhoto: empty\r\nProduct Model: Road-450\r\nList Price: 1457.99");
            //No match
            EnterCommand("pr x");
            WaitForOutput("x does not match any properties");

            //Invalid context
            CiceroUrl("home");
            EnterCommand("prop");
            WaitForOutput("The command: property is not available in the current context");

            //Multi-clause match
            CiceroUrl("object?object1=AdventureWorksModel.SalesPerson-284");
            WaitForOutput("Sales Person: Tete Mensa-Annan");
            EnterCommand("pr sales a");
            WaitForOutput("Sales Territory: Northwest\r\nSales Quota: 300000\r\nSales YTD: 1576562.1966\r\nSales Last Year: 0");
            EnterCommand("pr ter ory");
            WaitForOutput("Sales Territory: Northwest\r\nTerritory History (collection): 1 item");
            EnterCommand("pr sales z");
            WaitForOutput("sales z does not match any properties");

            //No fields
            CiceroUrl("object?object1=AdventureWorksModel.AddressType-2");
            WaitForOutput("Address Type: Home");
            EnterCommand("prop");
            WaitForOutput("No visible properties");

            //To many args
            EnterCommand("prop num,x");
            WaitForOutput("Too many arguments provided");

            //Reading properties in edit mode
            CiceroUrl("object?object1=AdventureWorksModel.Product-369&edit1=true&prop1_Style=%2522U%2520%2522&prop1_ListPrice=%2522500%2522");
            WaitForOutputStarting("Editing");
            EnterCommand("prop");
            WaitForOutputContaining("List Price: 500 (modified)");
            WaitForOutputContaining("Style: U  (modified)");

            //exact match takes priority over partial match
            //TODO: Need example from properties, not action params -  transfer this to Enter test
            //EnterCommand("field product category"); //which would also match product subcategory
            //WaitForOutput("Product Category: Bikes");

            //Check that enum property renders as text, not number
            CiceroUrl("object?object1=AdventureWorksModel.SalesOrderHeader-70996");
            WaitForOutput("Sales Order Header: SO70996");
            EnterCommand("pr status");
            WaitForOutput("Status: Shipped");
        }
        public virtual void Root()
        {
            CiceroUrl("object?object1=AdventureWorksModel.Product-459&collection1_ProductInventory=List");
            WaitForOutput("Collection: Product Inventory on Product: Lock Nut 19\r\n3 items");
            EnterCommand("root");
            WaitForOutput("Product: Lock Nut 19");

            //Invalid contexts
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("root");
            WaitForOutput("The command: root is not available in the current context");

            CiceroUrl("object?object1=AdventureWorksModel.Product-459");
            WaitForOutput("Product: Lock Nut 19");
            EnterCommand("root");
            WaitForOutput("The command: root is not available in the current context");

            //Argument added
            CiceroUrl("object?object1=AdventureWorksModel.Product-459&collection1_ProductInventory=List");
            WaitForOutput("Collection: Product Inventory on Product: Lock Nut 19\r\n3 items");
            EnterCommand("root x");
            WaitForOutput("Too many arguments provided");
        }
        public virtual void Save() {
            //Happy case
            CiceroUrl("object?object1=AdventureWorksModel.Product-839&edit1=true&prop1_ListPrice=%25221500%2522");
            WaitForOutput("Editing Product: HL Road Frame - Black, 48\r\n" +
                "Modified properties:\r\n" +
                "List Price: 1500");
            EnterCommand("save");
            WaitForOutput("Product: HL Road Frame - Black, 48");

            //Not valid once object is saved, or other contexts.
            EnterCommand("save");
            WaitForOutput("The command: save is not available in the current context");
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("save");
            WaitForOutput("The command: save is not available in the current context");
            CiceroUrl("object?object1=AdventureWorksModel.Customer-29688&dialog1=LastOrder");
            WaitForOutput("Customer: Handy Bike Services, AW00029688\r\nAction dialog: Last Order");
            EnterCommand("save");
            WaitForOutput("The command: save is not available in the current context");

            //No arguments
            CiceroUrl("object?object1=AdventureWorksModel.Product-839&edit1=true&prop1_ListPrice=%25221500%2522");
            WaitForOutput("Editing Product: HL Road Frame - Black, 48\r\n" +
                "Modified properties:\r\n" +
                "List Price: 1500");
            EnterCommand("save x");
            WaitForOutput("Too many arguments provided");

            //Field validation
            CiceroUrl("object?object1=AdventureWorksModel.WorkOrder-43134&edit1=true&prop1_OrderQty=%25220%2522");
            WaitForOutputStarting("Editing Work Order:");
            EnterCommand("save");
            WaitForOutput("Please complete or correct these fields:\r\n" +
                "Order Qty: 0 Order Quantity must be > 0");

            //Co-validation
            CiceroUrl("object?object1=AdventureWorksModel.WorkOrder-43133&edit1=true&prop1_StartDate=%252212%2520Jan%25202015%2522");
            WaitForOutputStarting("Editing Work Order:");
            EnterCommand("save");
            WaitForOutput("StartDate must be before EndDate");
        }
        public virtual void Show()
        {
            //Applied to List
            CiceroUrl("list?menu1=SpecialOfferRepository&action1=CurrentSpecialOffers");
            WaitForOutput("Result from Current Special Offers:\r\n16 items");
            EnterCommand("show 1");
            WaitForOutput("Item 1: No Discount");
            EnterCommand("show 16");
            WaitForOutput("Item 16: Mountain-500 Silver Clearance Sale");
            EnterCommand("show 2-4");
            WaitForOutput("Item 2: Volume Discount 11 to 14\r\nItem 3: Volume Discount 15 to 24\r\nItem 4: Volume Discount 25 to 40");
            EnterCommand("show 5-5");
            WaitForOutput("Item 5: Volume Discount 41 to 60");
            EnterCommand("show -3");
            WaitForOutput("Item 1: No Discount\r\nItem 2: Volume Discount 11 to 14\r\nItem 3: Volume Discount 15 to 24");
            EnterCommand("show 15-");
            WaitForOutput("Item 15: Half-Price Pedal Sale\r\nItem 16: Mountain-500 Silver Clearance Sale");


            //Invalid numbers
            EnterCommand("show 17");
            WaitForOutput("The highest numbered item is 16");
            EnterCommand("show 0");
            WaitForOutput("Item number or range values must be greater than zero");
            EnterCommand("show 15-17");
            WaitForOutput("The highest numbered item is 16");
            EnterCommand("show 0-3");
            WaitForOutput("Item number or range values must be greater than zero");
            EnterCommand("show 5-4");
            WaitForOutput("Starting item number cannot be greater than the ending item number");

            //Applied to collection
            CiceroUrl("object?object1=AdventureWorksModel.SalesOrderHeader-44518&selected1=256&collection1_Details=List");
            WaitForOutput("Collection: Details on Sales Order Header: SO44518\r\n20 items");
            EnterCommand("show 1");
            WaitForOutput("Item 1: 5 x Mountain-100 Black, 44");
            EnterCommand("show 20");
            WaitForOutput("Item 20: 2 x HL Mountain Frame - Black, 38");

            //No number
            EnterCommand("show");
            WaitForOutputStarting("Item 1: 5 x Mountain-100 Black, 44\r\nItem 2: 3 x Sport-100 Helmet, Black\r\nItem 3:");

            //Too many parms
            EnterCommand("show 4,5");
            WaitForOutput("Too many arguments provided");

            //Alpha parm
            EnterCommand("show one");
            WaitForOutput("one is not a number");

            //Invalid context
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("show 1");
            WaitForOutput("The command: show is not available in the current context");
            CiceroUrl("home?menu1=CustomerRepository");
            WaitForOutput("Customers menu");
            EnterCommand("show 1");
            WaitForOutput("The command: show is not available in the current context");
            CiceroUrl("object?object1=AdventureWorksModel.Customer-29863");
            WaitForOutput("Customer: Efficient Cycling, AW00029863");
            EnterCommand("show 1");
            WaitForOutput("The command: show is not available in the current context");

        }
        public virtual void Where()
        {
            CiceroUrl("home");
            CiceroUrl("object?object1=AdventureWorksModel.Product-358");
            WaitForOutput("Product: HL Grip Tape");
            //Do something to change the output
            EnterCommand("help");
            WaitForOutputStarting("Commands");
            EnterCommand("where");
            WaitForOutput("Product: HL Grip Tape");

            //Empty command == where
            EnterCommand("help");
            WaitForOutputStarting("Commands");
            ClearFieldThenType("input", Keys.Enter);
            WaitForOutput("Product: HL Grip Tape");

            //No arguments
            EnterCommand("where x");
            WaitForOutput("Too many arguments provided");
        }
        public virtual void SpaceBarAutoComplete()
        {
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            TypeIntoFieldWithoutClearing("input", "sel"+Keys.Space);
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "selection ");

            CiceroUrl("object?object1=AdventureWorksModel.Product-968");
            WaitForOutput("Product: Touring-1000 Blue, 54");
            //Hitting Tab with no entry has no effect
            TypeIntoFieldWithoutClearing("input", Keys.Space);
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "");
            WaitForOutput("Product: Touring-1000 Blue, 54");

            //Unrecognised two chars
            TypeIntoFieldWithoutClearing("input", "xx" + Keys.Space);
            WaitForOutput("No command begins with xx");

            //Single character
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            TypeIntoFieldWithoutClearing("input", "f" + Keys.Space);
            WaitForOutput("Command word must have at least 2 characters");

            //if there's any argument specified, just adds a space
            CiceroUrl("object?object1=AdventureWorksModel.Product-968");
            WaitForOutput("Product: Touring-1000 Blue, 54");
            TypeIntoFieldWithoutClearing("input", "he menu" + Keys.Space);
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "help menu ");

            //chained commands
            ClearFieldThenType("input", "me pr;ac rand;ok " + Keys.Space);
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "menu pr;action rand;ok ");

            //Space bar before command eventually removed
            ClearFieldThenType("input", " me pr; ac rand; ok " + Keys.Space);
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "menu pr;action rand;ok ");

        }
        public virtual void UnrecognisedCommand()
        {
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("m");
            WaitForOutput("Command word must have at least 2 characters");

            EnterCommand("hl");
            WaitForOutput("No command begins with hl");

        }
        public virtual void UpAndDownArrow()
        {
            CiceroUrl("home");
            EnterCommand("he");
            WaitForOutputStarting("Commands available");
            Assert.AreEqual("", WaitForCss("input").GetAttribute("value"));
            TypeIntoFieldWithoutClearing("input", Keys.ArrowUp);
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "help");
            TypeIntoFieldWithoutClearing("input", " gem" + Keys.Enter);
            WaitForOutputStarting("gemini command");
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "");
            TypeIntoFieldWithoutClearing("input", Keys.ArrowUp);
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "help gem");
            TypeIntoFieldWithoutClearing("input", Keys.ArrowDown);
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "");

            //TODO
            //CiceroUrl("home");
            //WaitForOutput("Welcome to Cicero");
            //EnterCommand("me pr; act rand; ok;show 1");
            //WaitForOutput("The command: show is not available in the current context");
            ////Test that the up arrow produces full string
            //Assert.AreEqual("", WaitForCss("input").GetAttribute("value"));
            //TypeIntoFieldWithoutClearing("input", Keys.ArrowUp);
            //wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value")
            //== "menu pr;action rand;ok;show 1");
        }
        public virtual void ScenarioEditAndSave()
        {
            //happy case -  edit one property
            CiceroUrl("object?object1=AdventureWorksModel.Product-838");
            WaitForOutput("Product: HL Road Frame - Black, 44");
            EnterCommand("prop list price");
            WaitForOutputStarting("List Price: ");
            var output = WaitForCss(".output").Text;
            var oldPrice = output.Split(' ').Last();
            EnterCommand("Edit");
            WaitForOutput("Editing Product: HL Road Frame - Black, 44");
            var rand = new Random();
            var newPrice = rand.Next(50, 150).ToString();
            EnterCommand("Enter list price, "+newPrice);
            WaitForOutput("Editing Product: HL Road Frame - Black, 44\r\n"+
                "Modified properties:\r\n"+
                "List Price: "+newPrice);
            EnterCommand("Save");
            WaitForOutput("Product: HL Road Frame - Black, 44");
            EnterCommand("prop list price");
            WaitForOutput("List Price: "+newPrice);

            //Updating a date and a link
            CiceroUrl("object?object1=AdventureWorksModel.WorkOrder-43132");
            WaitForOutputStarting("Work Order:");
            EnterCommand("edit");
            WaitForOutputStarting("Editing Work Order:");
            EnterCommand("enter end date,31 Dec 2015");
            WaitForOutputContaining("End Date: 31 Dec 2015");
            EnterCommand("enter scrap reas, gouge");
            WaitForOutputContaining("Scrap Reason: Gouge in metal");
            EnterCommand("save");
            WaitForOutputStarting("Work Order:");
            EnterCommand("prop end date");
            WaitForOutputContaining("2015"); // because Masks not being honoured
            EnterCommand("prop scrap rea");
            WaitForOutput("Scrap Reason: Gouge in metal");

            //Field validation
            //Order Quantity must be > 0
            CiceroUrl("object?object1=AdventureWorksModel.WorkOrder-43134");
            WaitForOutputStarting("Work Order:");
            EnterCommand("edit");
            WaitForOutputStarting("Editing Work Order:");
            EnterCommand("enter order qty,0");
            WaitForOutputContaining("Order Qty: 0");
            EnterCommand("save");
            WaitForOutput("Please complete or correct these fields:\r\n"+
                "Order Qty: 0 Order Quantity must be > 0");
   
            //Co-Validation
            CiceroUrl("object?object1=AdventureWorksModel.WorkOrder-43133");
            WaitForOutputStarting("Work Order:");
            EnterCommand("edit");
            WaitForOutputStarting("Editing Work Order:");
            EnterCommand("enter start date,17 Oct 2007"); //Seems to be necessary to clear the date fields fully
            WaitForOutputContaining("Start Date: 17 Oct 2007");
            EnterCommand("enter end date,15 Oct 2007");
            WaitForOutputContaining("End Date: 15 Oct 2007");
            EnterCommand("save");
            WaitForOutput("StartDate must be before EndDate");
            EnterCommand("enter end date,15 Oct 2008");
            WaitForOutputContaining("End Date: 15 Oct 2008");
            EnterCommand("save");
            WaitForOutputStarting("Work Order:");
        }
        public virtual void ScenarioMultiSelect()
        {
            //Multi-select and deselect - reference objects
            CiceroUrl("home?menu1=ProductRepository");
            WaitForOutput("Products menu");
            EnterCommand("action List Products By Sub Categories");
            WaitForOutputContaining("Action dialog: List Products By Sub Categories");
            WaitForOutputContaining("Sub Categories: -Mountain Bikes-Touring Bikes");
            EnterCommand("enter sub, handlebars");
            WaitForOutputContaining("Sub Categories: -Mountain Bikes-Touring Bikes-Handlebars");
            EnterCommand("enter sub, touring bikes");
            WaitForOutputContaining("Sub Categories: -Mountain Bikes-Handlebars");
            EnterCommand("enter sub, frames");
            WaitForOutput("Multiple matches:\r\nMountain Frames\r\nRoad Frames\r\nTouring Frames");
            EnterCommand("enter sub, x");
            WaitForOutput("None of the choices matches x");

            //Multi-select enums
            CiceroUrl("home?menu1=ProductRepository");
            WaitForOutput("Products menu");
            EnterCommand("action Find By Product Lines And Classes");
            WaitForOutputContaining("Product Line: M,S,");
            WaitForOutputContaining("Product Class: H,");
            EnterCommand("enter line, r");
            WaitForOutputContaining("Product Line: M,S,R");
            EnterCommand("enter line, m");
            WaitForOutputContaining("Product Line: S,R");
            EnterCommand("enter line, x");
            WaitForOutputContaining("None of the choices matches x");
            EnterCommand("enter line,");
            WaitForOutputContaining("Multiple matches:\r\nM\r\nR\r\nS\r\nT");
        }
        public virtual void ScenarioTransientObject() {
            //Happy case
            CiceroUrl("object?object1=AdventureWorksModel.Person-12044");
            WaitForOutput("Person: Gail Moore");
            EnterCommand("action Create New Credit Card");
            WaitForOutputContaining("Action dialog: Create New Credit Card");
            EnterCommand("ok");
            WaitForOutputStarting("Unsaved Credit Card");
            EnterCommand("enter card type, Vista");
            WaitForOutputContaining("Card Type: Vista");
            string number = DateTime.Now.Ticks.ToString(); //pseudo-random string
            var obfuscated = number.Substring(number.Length - 4).PadLeft(number.Length, '*');
            EnterCommand(" enter number,"+number);
            WaitForOutputContaining("Card Number: " + number);
            EnterCommand("enter month,12");
            WaitForOutputContaining("Exp Month: 12");
            EnterCommand("enter year,2020");
            WaitForOutputContaining("Exp Year: 2020");
            EnterCommand("save");
            WaitForOutput("Credit Card: "+obfuscated);
            //Incomplete fields
            CiceroUrl("object?object1=AdventureWorksModel.Person-12045");
            WaitForOutput("Person: Rakesh Tangirala");
            EnterCommand("action Create New Credit Card");
            WaitForOutputContaining("Action dialog: Create New Credit Card");
            EnterCommand("ok");
            WaitForOutput("Unsaved Credit Card");
            EnterCommand("save");
            WaitForOutputStarting("Please complete or correct these fields:");
        }
        public virtual void ScenarioUsingClipboard()
        {
            //Copy a Product to clipboard
            CiceroUrl("object?object1=AdventureWorksModel.Product-980");
            WaitForOutput("Product: Mountain-400-W Silver, 38");
            EnterCommand("clip copy");
            WaitForOutput("Clipboard contains: Product: Mountain-400-W Silver, 38");
            EnterCommand("menu emp");
            WaitForOutput("Employees menu");
            EnterCommand("ac create");
            WaitForOutput("Employees menu\r\nAction dialog: Create New Employee From Contact\r\nContact Details: empty");
            EnterCommand("enter details, paste");
            WaitForOutput("Contents of Clipboard are not compatible with the field");
            EnterCommand("clip show");
            WaitForOutput("Clipboard contains: Product: Mountain-400-W Silver, 38");
            EnterCommand("clip discard");
            WaitForOutput("Clipboard is empty");
            EnterCommand("enter details, paste");
            WaitForOutput("Cannot use Clipboard as it is empty");
            CiceroUrl("object?object1=AdventureWorksModel.Person-7185");
            WaitForOutput("Person: Carmen Perez");
            EnterCommand("clip copy");
            WaitForOutput("Clipboard contains: Person: Carmen Perez");
            EnterCommand("back");
            WaitForOutput("Employees menu\r\nAction dialog: Create New Employee From Contact\r\nContact Details: empty");
            EnterCommand("enter details, paste");
            WaitForOutput("Employees menu\r\nAction dialog: Create New Employee From Contact\r\nContact Details: Carmen Perez");
            EnterCommand("ok");
            WaitForOutput("Unsaved Employee");
        }
        public virtual void ChainedCommands()
        {
            //Happy case
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("menu pr; action rand; ok");
            WaitForOutputStarting("Product:");

            //Try to chain a command that may never be chained
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("menu pr; action rand; ok; edit");
            WaitForOutputStarting("edit command may not be chained. Use Where command to see where execution stopped.");
            EnterCommand("where");
            WaitForOutputStarting("Product:");

            //Try to chain an action invocation on a non-query action
            EnterCommand("menu special; ac create; ok");
            WaitForOutputStarting("ok command may not be chained unless the action is query-only. Use Where command to see where execution stopped.");
            EnterCommand("where");
            WaitForOutputStarting("Special Offers menu\r\nAction dialog: Create New Special Offer");

            //Try to chain a command that is not avialable in the current context
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("menu pr;action rand;ok;show 1");
            WaitForOutput("The command: show is not available in the current context");


            //Error in execution -  Timing problem?
            //CiceroUrl("home");
            //WaitForOutput("Welcome to Cicero");
            //EnterCommand("menu special; ac current; ok; show 20");
            //WaitForOutput("The highest numbered item is 16");
            //EnterCommand("where");
            //WaitForOutput("Result from Current Special Offers:\r\n16 items");
        }
    }
    public abstract class CiceroTests : CiceroTestRoot
    {
        [TestMethod]
        public override void Action() { base.Action(); }
        [TestMethod]
        public override void BackAndForward() { base.BackAndForward(); }
        [TestMethod]
        public override void Cancel() { base.Cancel(); }
        [TestMethod]
        public override void Clipboard() { base.Clipboard(); }
        [TestMethod]
        public override void Edit() { base.Edit(); }
        [TestMethod]
        public override void Enter() { base.Enter(); }
        [TestMethod]
        public override void Gemini() { base.Gemini(); }
        [TestMethod]
        public override void Goto() { base.Goto(); }
        [TestMethod]
        public override void Help() { base.Help(); }
        [TestMethod]
        public override void Menu() { base.Menu(); }
        [TestMethod]
        public override void OK() { base.OK(); }
        [TestMethod]
        public override void Page() { base.Page(); }
        [TestMethod]
        public override void Property() { base.Property(); }
        [TestMethod]
        public override void Root() { base.Root(); }
        [TestMethod]
        public override void Save() { base.Save(); }
        [TestMethod]
        public override void Show() { base.Show(); }
        [TestMethod]
        public override void Where() { base.Where(); }
        [TestMethod]
        public override void SpaceBarAutoComplete() { base.SpaceBarAutoComplete(); }
        [TestMethod]
        public override void UnrecognisedCommand() { base.UnrecognisedCommand(); }
        [TestMethod]
        public override void UpAndDownArrow() { base.UpAndDownArrow(); }
        [TestMethod]
        public override void ScenarioEditAndSave() { base.ScenarioEditAndSave(); }
        [TestMethod]
        public override void ScenarioMultiSelect() { base.ScenarioMultiSelect(); }
        [TestMethod]
        public override void ScenarioTransientObject() { base.ScenarioTransientObject(); }
        [TestMethod]
        public override void ScenarioUsingClipboard() { base.ScenarioUsingClipboard(); }
        [TestMethod]
        public override void ChainedCommands() { base.ChainedCommands(); }
    }

    #region Individual tests - browser specific 

    // [TestClass, Ignore]
    public class CiceroTestsIe : CiceroTests
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context)
        {
            FilePath(@"drivers.IEDriverServer.exe");
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest()
        {
            InitIeDriver();
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            base.CleanUpTest();
        }
    }

    //[TestClass] //Comment out if MegaTest is commented in
    public class CiceroTestsFirefox : CiceroTests
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context)
        {
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest()
        {
            InitFirefoxDriver();
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            base.CleanUpTest();
        }
    }

    //[TestClass, Ignore]
    public class CiceroTestsChrome : CiceroTests
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context)
        {
            FilePath(@"drivers.chromedriver.exe");
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest()
        {
            InitChromeDriver();
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            base.CleanUpTest();
        }

        protected override void ScrollTo(IWebElement element)
        {
            string script = string.Format("window.scrollTo(0, {0})", element.Location.Y);
            ((IJavaScriptExecutor)br).ExecuteScript(script);
        }
    }

    #endregion

    #region Mega tests
    public abstract class MegaCiceroTestsRoot : CiceroTestRoot
    {
        [TestMethod]
        public void MegaCiceroTests()
        {

            base.Action();
            base.BackAndForward();
            base.Cancel();
            base.Clipboard();
            base.Edit();
            base.Enter();
            base.Gemini();
            base.Goto();
            base.Help();
            base.Menu();
            base.OK();
            base.Page();
            base.Property();
            base.Root();
            base.Save(); 
            base.Show();
            base.Where();
            base.SpaceBarAutoComplete();
            base.UnrecognisedCommand();
            base.UpAndDownArrow();
            base.ScenarioEditAndSave();
            base.ScenarioMultiSelect();
            base.ScenarioTransientObject();
            base.ScenarioUsingClipboard();
            base.ChainedCommands();
        }
    }
    [TestClass]
    public class MegaCiceroTestsFirefox : MegaCiceroTestsRoot
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context)
        {
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest()
        {
            InitFirefoxDriver();
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            base.CleanUpTest();
        }
    }
    #endregion
}