// using System;
// using Xunit;
// using BarLib;
// using System.Collections.Generic;
// using Moq;
// using Autofac.Extras.Moq;
// using FluentAssertions;
// using System.Linq;
// using System.Threading.Tasks;

// namespace BarLib.Tests
// {
//     public class UnitTest1
//     {
//         [Fact]
//         public async Task Test1()
//         {
//             var user = new UserBar
//             {
//                 UserId = Guid.NewGuid().ToString(),
//                 AvailableIngredients = new List<string>{"1", "3","7"},
//             };

//             var mock = AutoMock.GetLoose();
//             mock.Mock<IStorageContext<Drink>>().Setup(x=>x.GetAsync()).ReturnsAsync(Samples.GetDrinks().ToList());
            
//             var gen = mock.Create<LibraryGenerator>();
//             var drinks = await gen.BuildAsync(user);

//             drinks.Should().HaveCount(1);
//         }
//     }

//     public class Samples
//     {
//         public static IEnumerable<Ingredient> GetIngredients() =>
//         new List<Ingredient>{
//             new Ingredient("1","Whiskey", IngredientType.Spirit),
//             new Ingredient("2", "Vodka", IngredientType.Spirit),
//             new Ingredient("3", "Sweet Vermouth", IngredientType.Adjunct),
//             new Ingredient("4", "Gin", IngredientType.Spirit),
//             new Ingredient("5", "Tonic Water", IngredientType.Adjunct),
//             new Ingredient("6", "Lime Juice", IngredientType.Adjunct),
//             new Ingredient("7", "Bitters", IngredientType.Adjunct),
//         };

//         public static IEnumerable<Drink> GetDrinks() => new List<Drink>{
//             new Drink("1","Manhattan"){Steps = new List<DrinkStep>{
//                 new DrinkStep("1",2,"oz"),
//                 new DrinkStep("3", 0.25f, "oz"),
//                 new DrinkStep("7", 1, "Dash")
//             }},
//             new Drink("2", "Gin & Tonic"){Steps = new List<DrinkStep>{
//                 new DrinkStep("4",2, "oz"),
//                 new DrinkStep("5", 2, "oz"),
//             }},
//         };


//     }
// }
