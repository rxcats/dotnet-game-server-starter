using System;
using Xunit;

namespace RxCats.GameData.Tests
{
    public class DatabaseContextTest
    {
        private readonly DatabaseContext db;

        public DatabaseContextTest()
        {
            db = new DatabaseContext("Server=localhost;Uid=developer;Pwd=qwer1234;Database=test;");
        }

        [Fact]
        public async void AddTest()
        {
            await db.AddAsync(new Person
            {
                Id = DateTime.Now.ToFileTime(),
                Name = "test",
                Number = 0
            });

            int save = await db.SaveChangesAsync();

            Assert.True(save == 1);
        }
    }
}
