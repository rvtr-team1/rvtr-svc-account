﻿using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RVTR.Account.DataContext;
using RVTR.Account.DataContext.Repositories;
using RVTR.Account.ObjectModel.Models;
using Xunit;

namespace RVTR.Account.UnitTesting.Tests
{
  public class RepositoryTest
  {
    private static readonly SqliteConnection _connection = new SqliteConnection("Data Source=:memory:");
    private static readonly DbContextOptions<AccountContext> _options = new DbContextOptionsBuilder<AccountContext>().UseSqlite(_connection).Options;

    public static readonly IEnumerable<object[]> _records = new List<object[]>()
    {
      new object[]
      {
        new AccountModel() { Id = 1, Name = "name" },
        new ProfileModel() { Id = 1, Email = "email" },
      }
    };

    [Theory]
    [MemberData(nameof(_records))]
    public async void Test_Repository_DeleteAsync(AccountModel lodging, ProfileModel profile)
    {
      await _connection.OpenAsync();

      try
      {
        using (var ctx = new AccountContext(_options))
        {
          await ctx.Database.EnsureCreatedAsync();
          await ctx.Accounts.AddAsync(lodging);
          await ctx.Profiles.AddAsync(profile);
          await ctx.SaveChangesAsync();
        }

        using (var ctx = new AccountContext(_options))
        {
          var lodgings = new Repository<AccountModel>(ctx);

          await lodgings.DeleteAsync(1);
          await ctx.SaveChangesAsync();

          Assert.Empty(await ctx.Accounts.ToListAsync());
        }

        using (var ctx = new AccountContext(_options))
        {
          var profiles = new Repository<ProfileModel>(ctx);

          await profiles.DeleteAsync(1);
          await ctx.SaveChangesAsync();

          Assert.Empty(await ctx.Profiles.ToListAsync());
        }
      }
      finally
      {
        await _connection.CloseAsync();
      }
    }

    [Theory]
    [MemberData(nameof(_records))]
    public async void Test_Repository_InsertAsync(AccountModel lodging, ProfileModel profile)
    {
      await _connection.OpenAsync();

      try
      {
        using (var ctx = new AccountContext(_options))
        {
          await ctx.Database.EnsureCreatedAsync();
        }

        using (var ctx = new AccountContext(_options))
        {
          var lodgings = new Repository<AccountModel>(ctx);

          await lodgings.InsertAsync(lodging);
          await ctx.SaveChangesAsync();

          Assert.NotEmpty(await ctx.Accounts.ToListAsync());
        }

        using (var ctx = new AccountContext(_options))
        {
          var profiles = new Repository<ProfileModel>(ctx);

          await profiles.InsertAsync(profile);
          await ctx.SaveChangesAsync();

          Assert.NotEmpty(await ctx.Profiles.ToListAsync());
        }
      }
      finally
      {
        await _connection.CloseAsync();
      }
    }

    [Fact]
    public async void Test_Repository_SelectAsync()
    {
      await _connection.OpenAsync();

      try
      {
        using (var ctx = new AccountContext(_options))
        {
          await ctx.Database.EnsureCreatedAsync();
        }

        using (var ctx = new AccountContext(_options))
        {
          var lodgings = new Repository<AccountModel>(ctx);

          var actual = await lodgings.SelectAsync();

          Assert.Empty(actual);
        }

        using (var ctx = new AccountContext(_options))
        {
          var profiles = new Repository<ProfileModel>(ctx);

          var actual = await profiles.SelectAsync();

          Assert.Empty(actual);
        }
      }
      finally
      {
        await _connection.CloseAsync();
      }
    }

    [Fact]
    public async void Test_Repository_SelectAsync_ById()
    {
      await _connection.OpenAsync();

      try
      {
        using (var ctx = new AccountContext(_options))
        {
          await ctx.Database.EnsureCreatedAsync();
        }

        using (var ctx = new AccountContext(_options))
        {
          var lodgings = new Repository<AccountModel>(ctx);

          var actual = await lodgings.SelectAsync(1);

          Assert.Null(actual);
        }

        using (var ctx = new AccountContext(_options))
        {
          var profiles = new Repository<ProfileModel>(ctx);

          var actual = await profiles.SelectAsync(1);

          Assert.Null(actual);
        }
      }
      finally
      {
        await _connection.CloseAsync();
      }
    }

    [Theory]
    [MemberData(nameof(_records))]
    public async void Test_Repository_Update(AccountModel lodging, ProfileModel profile)
    {
      await _connection.OpenAsync();

      try
      {
        using (var ctx = new AccountContext(_options))
        {
          await ctx.Database.EnsureCreatedAsync();
          await ctx.Accounts.AddAsync(lodging);
          await ctx.Profiles.AddAsync(profile);
          await ctx.SaveChangesAsync();
        }

        using (var ctx = new AccountContext(_options))
        {
          var lodgings = new Repository<AccountModel>(ctx);
          var expected = await ctx.Accounts.FirstAsync();

          expected.Name = "name";
          lodgings.Update(expected);
          await ctx.SaveChangesAsync();

          var actual = await ctx.Accounts.FirstAsync();

          Assert.Equal(expected, actual);
        }

        using (var ctx = new AccountContext(_options))
        {
          var profiles = new Repository<ProfileModel>(ctx);
          var expected = await ctx.Profiles.FirstAsync();

          expected.Email = "email";
          profiles.Update(expected);
          await ctx.SaveChangesAsync();

          var actual = await ctx.Profiles.FirstAsync();

          Assert.Equal(expected, actual);
        }
      }
      finally
      {
        await _connection.CloseAsync();
      }
    }
  }
}
