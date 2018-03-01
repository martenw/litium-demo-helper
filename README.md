# LitiumDemoHelper

This project is an implementation of a [Litium Test Project](https://kc.litiumstudio.se/documentation/get-started/setting_up_a_test_project) with utilities that can be used to quickly get a Litium Demo environment set up.

[Description on usage can be found in this series of blogposts](https://blog.gooser.se/post/170796148634/setting-up-a-litium-customer-demo-part-1-excel)

## Getting started

1. Add the project to your accelerator solution
1. Rename App.template.config > App.config
1. Adjust FoundationConnectionString of App.config to that of your website
1. Modify the tests and run them individually to modify your solution

## Avaliable tests:

#### ```Litium.Accelerator.Demo.PimDataModel.CreateFields()```
Used to quickly setup fields in existing field templateds in 
PIM. Chreate `InlineData` for each field that should be 
generated. Sample field definitions can be found in the codefile.

#### ```Litium.Accelerator.Demo.ProductInformation.MoveProductsIntoCategories()```
Create the a PIM Category structure by reading category information found as field data on the products. It is 
necessary to modify the method `GetCategoryTrees()`before 
running the method to set up how category information should
be read from the product.

#### ```Litium.Accelerator.Demo.ProductInformation.PublishEverything()```
Does what it sounds like, publishes everything everywhere. Or more specifically connects every category and variant to every website.
