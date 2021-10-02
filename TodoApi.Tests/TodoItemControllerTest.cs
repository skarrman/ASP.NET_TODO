using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Controllers;
using TodoApi.Models;
using Xunit;

namespace TodoApi.Tests
{
    public class TodoItemControllerTest: TodoItemTestContextSetup
    {
        public TodoItemControllerTest()
            : base(new DbContextOptionsBuilder<TodoContext>()
                  .UseInMemoryDatabase(databaseName: "Test")
              .Options)
        {
        }

        [Fact]
        public async void Can_Get_All_Items()
        {
            using (var context = new TodoContext(ContextOptions))
            {
                var controller = new TodoItemsController(context);

                var response = await controller.GetTodoItems();

                Assert.IsType<OkObjectResult>(response.Result);

                var result = (OkObjectResult)response.Result;
                var items = ((IEnumerable<TodoItem>)result.Value).ToList();

                Assert.Equal(3, items.Count());
            }
        }

        [Fact]
        public async void Can_Get_One_Existing_Item()
        {
            using (var context = new TodoContext(ContextOptions))
            {
                var controller = new TodoItemsController(context);

                var response = await controller.GetTodoItem(1);

                Assert.IsType<OkObjectResult>(response.Result);

                var result = (OkObjectResult)response.Result;
                var item = ((TodoItem)result.Value);

                Assert.Equal(1, item.Id);
            }
        }

        [Fact]
        public async void Get_NotFound_When_Getting_Nonexistent_Item()
        {
            using (var context = new TodoContext(ContextOptions))
            {
                var controller = new TodoItemsController(context);

                var response = await controller.GetTodoItem(4);

                Assert.IsType<NotFoundResult>(response.Result);
            }
        }

        [Fact]
        public async void Can_Put_Existing_Item()
        {
            using (var context = new TodoContext(ContextOptions))
            {
                var controller = new TodoItemsController(context);

                var newItem = new TodoItem
                {
                    Id = 2,
                    Name = "Take out trash",
                    IsComplete = false,
                };

                var response = await controller.PutTodoItem(2, newItem);

                Assert.IsType<NoContentResult>(response);

                var updatedItem = context.TodoItems
                    .Where(item => item.Id == newItem.Id)
                    .FirstOrDefault();
                Assert.Equal(newItem, updatedItem);
            }
        }

        [Fact]
        public async void Get_Bad_Request_When_Put_With_Wrong_Id()
        {
            using (var context = new TodoContext(ContextOptions))
            {
                var controller = new TodoItemsController(context);

                var newItem = new TodoItem
                {
                    Id = 2,
                };

                var response = await controller.PutTodoItem(3, newItem);

                Assert.IsType<BadRequestResult>(response);

            }
        }

        [Fact]
        public async void Get_Not_Found_When_Put_With_Nonexistent_Item()
        {
            using (var context = new TodoContext(ContextOptions))
            {
                var controller = new TodoItemsController(context);

                var newItem = new TodoItem
                {
                    Id = 4,
                };

                var response = await controller.PutTodoItem(4, newItem);

                Assert.IsType<NotFoundResult>(response);

                var nullItem = context.TodoItems
                    .Where(item => item.Id == newItem.Id)
                    .FirstOrDefault();

                Assert.Null(nullItem);
            }
        }

        [Fact]
        public async void Can_Post_New_Item()
        {
            using (var context = new TodoContext(ContextOptions))
            {
                var controller = new TodoItemsController(context);

                var newItem = new TodoItem
                {
                    Name = "Write unit tests",
                    IsComplete = false,
                };

                var response = await controller.PostTodoItem(newItem);

                Assert.IsType<CreatedAtActionResult>(response.Result);

                var item = (TodoItem)((CreatedAtActionResult)response.Result).Value;

                Assert.Equal(newItem.Name, item.Name);
                Assert.Equal(newItem.IsComplete, item.IsComplete);

                var dbItem = context.TodoItems
                    .Where(_item => _item.Id == item.Id)
                    .FirstOrDefault();

                Assert.Equal(item, dbItem);
            }
        }

        [Fact]
        public async void Can_Delete_Existing_Item()
        {
            using (var context = new TodoContext(ContextOptions))
            {
                var controller = new TodoItemsController(context);

                var deleteId = 1;

                var response = await controller.DeleteTodoItem(deleteId);

                Assert.IsType<NoContentResult>(response);

                var nullItem = context.TodoItems
                    .Where(item => item.Id == deleteId)
                    .FirstOrDefault();

                Assert.Null(nullItem);
            }
        }

        [Fact]
        public async void Get_Not_Found_When_Delete_Nonexistent_Item()
        {
            using (var context = new TodoContext(ContextOptions))
            {
                var controller = new TodoItemsController(context);

                var deleteId = 4;

                var response = await controller.DeleteTodoItem(deleteId);

                Assert.IsType<NotFoundResult>(response);

                Assert.Equal(3, context.TodoItems.Count());
            }
        }
    }
}
