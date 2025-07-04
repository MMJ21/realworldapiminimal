﻿using Microsoft.EntityFrameworkCore;
using Realworlddotnet.Core.Entities;

namespace Realworlddotnet.Data.Contexts;

public class ConduitContext(DbContextOptions<ConduitContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Article> Articles { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Tag> Tags { get; set; }

    public DbSet<UserLink> FollowedUsers { get; set; }
    public DbSet<ArticleFavorite> ArticleFavorites { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Username);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasMany(x => x.ArticleComments)
                .WithOne(x => x.Author);
        });

        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.Ignore(e => e.Favorited);
            entity.Ignore(e => e.FavoritesCount);
        });

        modelBuilder.Entity<ArticleFavorite>(entity =>
        {
            entity.HasKey(e => new { e.ArticleId, UserId = e.Username });
            entity.HasOne(x => x.Article).WithMany(x => x.ArticleFavorites)
                .HasForeignKey(x => x.ArticleId)
                .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.User).WithMany(x => x.ArticleFavorites);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasOne(x => x.Article)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Author)
                .WithMany(x => x.ArticleComments)
                .HasForeignKey(x => x.Username)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<UserLink>(entity =>
        {
            entity.HasKey(x => new { x.Username, x.FollowerUsername });
            entity.HasOne(x => x.User)
                .WithMany(x => x.Followers)
                .HasForeignKey(x => x.Username);

            entity.HasOne(x => x.FollowerUser)
                .WithMany(x => x.FollowedUsers)
                .HasForeignKey(x => x.FollowerUsername)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }
}
