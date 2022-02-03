using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace Disc.Bot
{
    // note that in here we explicitly ask for duration. This is optional,
    // since we set the defaults.
    public class Commands : BaseCommandModule
    {
        System.Runtime.Caching.MemoryCache cache = new System.Runtime.Caching.MemoryCache("CacheName");
        [Command("rep"), Description("Rep a user."), Aliases("repped")]
        public async Task Rep(CommandContext ctx, [Description("Rep a faggot.")] DiscordMember member, [RemainingText]string reason) // this command takes a member as an argument; you can pass one by username, nickname, id, or mention
        {
            if (cache.Contains(ctx.Message.Author.Id.ToString()))
            {
                if (Convert.ToInt32(ProcessReps.balance(ctx.Message.Author.Id.ToString())) > 10)
                {
                    Console.WriteLine("Reps over 10");
                    if (ProcessReps.RandomNumber(0, 100) < 50)
                    {
                        // first retrieve the interactivity module from the client
                        var interactivity = ctx.Client.GetInteractivity();
                        var emoji = DiscordEmoji.FromName(ctx.Client, ":rep:");
                        await ctx.RespondAsync($"React with {emoji} to bypass cooldown, some reps may be deducted.");
                        var em = await interactivity.WaitForReactionAsync(xe => xe.Emoji == emoji, ctx.User, TimeSpan.FromSeconds(60));
                        if (!em.TimedOut)
                        {
                            ProcessReps.deductCredits(ctx.Message.Author.Id.ToString(), ProcessReps.RandomNumber(0, 10).ToString());
                            string reps = ProcessReps.addCredits(member.Id.ToString(), "1");
                            string target = member.Mention;
                            await ctx.TriggerTypingAsync();
                            var green_role = ctx.Guild.Roles.Values.FirstOrDefault(x => x.Name == "green");
                            var red_role = ctx.Guild.Roles.Values.FirstOrDefault(x => x.Name == "red");
                            emoji = DiscordEmoji.FromName(ctx.Client, ":rep:");
                            var emoji_string = "";
                            if (Convert.ToInt32(reps) > 0)
                            {
                                for (int i = 0; i < Convert.ToInt32(reps); i++)
                                {
                                    if (i < 10)
                                        emoji_string += emoji;
                                }
                            }
                            else if (Convert.ToInt32(reps) < 0)
                            {
                                int counter = Convert.ToInt32(reps) * -1;
                                emoji = DiscordEmoji.FromName(ctx.Client, ":neg:");
                                for (int i = 0; i < counter; i++)
                                {
                                    if (i < 10)
                                        emoji_string += emoji;
                                }
                                //emoji_string = emoji + emoji + emoji + emoji;
                            }
                            else
                            {
                                emoji_string += emoji;
                            }
                            var newColor = new DiscordColor(DiscordColor.Green.Value);
                            if (Convert.ToInt32(reps) < 0)
                            {
                                newColor = new DiscordColor(DiscordColor.Red.Value);
                                await member.GrantRoleAsync(red_role);
                                await member.RevokeRoleAsync(green_role);
                            }
                            else
                            {
                                newColor = new DiscordColor(DiscordColor.Green.Value);
                                await member.GrantRoleAsync(green_role);
                                await member.RevokeRoleAsync(red_role);
                            }
                            string descriptio = "Has repped " + target;
                            if (ProcessReps.RandomNumber(1, 100) == 69)
                            {
                                descriptio = "Has smooched " + target;
                                reps = ProcessReps.addCredits(member.Id.ToString(), "2");

                            }
                            if (reason != null)
                            {
                                descriptio = descriptio + " Reason: " + reason;
                            }
                            var embed = new DiscordEmbedBuilder
                            {
                                Color = newColor,
                                Description = descriptio,
                                Author = new DiscordEmbedBuilder.EmbedAuthor
                                {
                                    Name = ctx.Message.Author.Username,
                                    IconUrl = ctx.Message.Author.AvatarUrl
                                },
                            };
                            embed.AddField("Total Reps: " + reps, emoji_string);
                            await ctx.RespondAsync(embed);
                        }
                    }
                    else
                    {
                        await ctx.TriggerTypingAsync();
                        await ctx.RespondAsync(ctx.Message.Author.Mention + " slowdown phaggot.");
                    }
                }
                else
                {
                    await ctx.TriggerTypingAsync();
                    await ctx.RespondAsync(ctx.Message.Author.Mention + " slowdown phaggot.");
                    if (ProcessReps.RandomNumber(1, 10) == 1)
                    {
                        string reps = ProcessReps.deductCredits(ctx.Message.Author.Id.ToString(), "1");
                        string target = ctx.Message.Author.Mention;
                        await ctx.TriggerTypingAsync();

                        var green_role = ctx.Guild.Roles.Values.FirstOrDefault(x => x.Name == "green");
                        var red_role = ctx.Guild.Roles.Values.FirstOrDefault(x => x.Name == "red");
                        var emoji = DiscordEmoji.FromName(ctx.Client, ":rep:");
                        var emoji_string = "";
                        if (Convert.ToInt32(reps) > 0)
                        {
                            for (int i = 0; i < Convert.ToInt32(reps); i++)
                            {
                                if (i < 10)
                                    emoji_string += emoji;
                            }
                        }
                        else if (Convert.ToInt32(reps) < 0)
                        {
                            int counter = Convert.ToInt32(reps) * -1;
                            emoji = DiscordEmoji.FromName(ctx.Client, ":neg:");
                            for (int i = 0; i < counter; i++)
                            {
                                if (i < 10)
                                    emoji_string += emoji;
                            }
                            //emoji_string = emoji + emoji + emoji + emoji;
                        }
                        var newColor = new DiscordColor(DiscordColor.Green.Value);
                        if (Convert.ToInt32(reps) < 0)
                        {
                            newColor = new DiscordColor(DiscordColor.Red.Value);
                            await member.GrantRoleAsync(red_role);
                            await member.RevokeRoleAsync(green_role);
                        }
                        else
                        {
                            newColor = new DiscordColor(DiscordColor.Green.Value);
                            await member.GrantRoleAsync(green_role);
                            await member.RevokeRoleAsync(red_role);
                        }
                        string descriptio = "Has negged " + target;
                        var embed = new DiscordEmbedBuilder
                        {
                            Color = newColor,
                            Description = descriptio,
                            Author = new DiscordEmbedBuilder.EmbedAuthor
                            {
                                Name = ctx.Client.CurrentUser.Username,
                                IconUrl = ctx.Client.CurrentUser.AvatarUrl
                            },
                        };
                        embed.AddField("Total Reps: " + reps, emoji_string);
                        await ctx.RespondAsync(embed);
                    }
                }
            }
            else if (ctx.Message.Author.Id == member.Id)
            {
                await ctx.TriggerTypingAsync();
                await ctx.RespondAsync(ctx.Message.Author.Mention + " nope.");
            }
            else
            {
                cache.Add(ctx.Message.Author.Id.ToString(), ctx.Message.Author.Username.ToString(), new CacheItemPolicy()
                { AbsoluteExpiration = DateTime.UtcNow.AddMinutes(15) });
                //Start multiplier based on reppers rep count
                string reps = "";
                if (Convert.ToInt32(ProcessReps.balance(ctx.Message.Author.Id.ToString())) > 100)
                {
                    reps = ProcessReps.addCredits(member.Id.ToString(), "2");
                }
                else
                {
                    reps = ProcessReps.addCredits(member.Id.ToString(), "1");
                }
                string target = member.Mention;
                await ctx.TriggerTypingAsync();

                var green_role = ctx.Guild.Roles.Values.FirstOrDefault(x => x.Name == "green");
                var red_role = ctx.Guild.Roles.Values.FirstOrDefault(x => x.Name == "red");
                var emoji = DiscordEmoji.FromName(ctx.Client, ":rep:");
                var emoji_string = "";
                if (Convert.ToInt32(reps) > 0)
                {
                    for (int i = 0; i < Convert.ToInt32(reps); i++)
                    {
                        if (i < 10)
                            emoji_string += emoji;
                    }
                }
                else if (Convert.ToInt32(reps) < 0)
                {
                    int counter = Convert.ToInt32(reps) * -1;
                    emoji = DiscordEmoji.FromName(ctx.Client, ":neg:");
                    for (int i = 0; i < counter; i++)
                    {
                        if (i < 10)
                            emoji_string += emoji;
                    }
                    //emoji_string = emoji + emoji + emoji + emoji;
                }
                var newColor = new DiscordColor(DiscordColor.Green.Value);
                if (Convert.ToInt32(reps) < 0)
                {
                    newColor = new DiscordColor(DiscordColor.Red.Value);
                    await member.GrantRoleAsync(red_role);
                    await member.RevokeRoleAsync(green_role);
                }
                else
                {
                    newColor = new DiscordColor(DiscordColor.Green.Value);
                    await member.GrantRoleAsync(green_role);
                    await member.RevokeRoleAsync(red_role);
                }
                string descriptio = "Has repped " + target;
                if(ProcessReps.RandomNumber(1,1000) == 69)
                    {
                    descriptio = "Has smooched " + target;
                    reps = ProcessReps.addCredits(member.Id.ToString(), "2");
                }
                if (reason != null)
                {
                    descriptio = descriptio + " Reason: " + reason;
                }
                var embed = new DiscordEmbedBuilder
                {
                    Color = newColor,
                    Description = descriptio,
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = ctx.Message.Author.Username,
                        IconUrl = ctx.Message.Author.AvatarUrl
                    },
                };
                embed.AddField("Total Reps: " + reps, emoji_string);
                await ctx.RespondAsync(embed);
            }
        }
        [Command("neg"), Description("Neg a user."), Aliases("negged")]
        public async Task Neg(CommandContext ctx, [Description("Neg a faggot.")] DiscordMember member, [RemainingText]string reason) // this command takes a member as an argument; you can pass one by username, nickname, id, or mention
        {
            if (cache.Contains(ctx.Message.Author.Id.ToString()))
            {
                if (Convert.ToInt32(ProcessReps.balance(ctx.Message.Author.Id.ToString())) > 10)
                {
                    if (ProcessReps.RandomNumber(0, 100) < 50)
                    {
                        // first retrieve the interactivity module from the client
                        var interactivity = ctx.Client.GetInteractivity();
                        var emoji = DiscordEmoji.FromName(ctx.Client, ":rep:");
                        await ctx.RespondAsync($"React with {emoji} to bypass cooldown, some reps may be deducted.");
                        var em = await interactivity.WaitForReactionAsync(xe => xe.Emoji == emoji, ctx.User, TimeSpan.FromSeconds(60));
                        if (!em.TimedOut)
                        {
                            ProcessReps.deductCredits(ctx.Message.Author.Id.ToString(), ProcessReps.RandomNumber(0, 10).ToString());
                            string reps = ProcessReps.deductCredits(member.Id.ToString(), "1");
                            string target = member.Mention;
                            await ctx.TriggerTypingAsync();
                            var green_role = ctx.Guild.Roles.Values.FirstOrDefault(x => x.Name == "green");
                            var red_role = ctx.Guild.Roles.Values.FirstOrDefault(x => x.Name == "red");
                            emoji = DiscordEmoji.FromName(ctx.Client, ":rep:");
                            var emoji_string = "";
                            if (Convert.ToInt32(reps) > 0)
                            {
                                for (int i = 0; i < Convert.ToInt32(reps); i++)
                                {
                                    if (i < 10)
                                        emoji_string += emoji;
                                }
                            }
                            else if (Convert.ToInt32(reps) < 0)
                            {
                                int counter = Convert.ToInt32(reps) * -1;
                                emoji = DiscordEmoji.FromName(ctx.Client, ":neg:");
                                for (int i = 0; i < counter; i++)
                                {
                                    if (i < 10)
                                        emoji_string += emoji;
                                }
                                //emoji_string = emoji + emoji + emoji + emoji;
                            }
                            else
                            {
                                emoji_string += emoji;
                            }
                            var newColor = new DiscordColor(DiscordColor.Green.Value);
                            if (Convert.ToInt32(reps) < 0)
                            {
                                newColor = new DiscordColor(DiscordColor.Red.Value);
                                await member.GrantRoleAsync(red_role);
                                await member.RevokeRoleAsync(green_role);
                            }
                            else
                            {
                                newColor = new DiscordColor(DiscordColor.Green.Value);
                                await member.GrantRoleAsync(green_role);
                                await member.RevokeRoleAsync(red_role);
                            }
                            string descriptio = "Has negged " + target;
                            if (ProcessReps.RandomNumber(1, 100) == 69)
                            {
                                descriptio = "Has pegged " + target;
                                reps = ProcessReps.deductCredits(member.Id.ToString(), "2");

                            }
                            if (reason != null)
                            {
                                descriptio = descriptio + " Reason: " + reason;
                            }
                            var embed = new DiscordEmbedBuilder
                            {
                                Color = newColor,
                                Description = descriptio,
                                Author = new DiscordEmbedBuilder.EmbedAuthor
                                {
                                    Name = ctx.Message.Author.Username,
                                    IconUrl = ctx.Message.Author.AvatarUrl
                                },
                            };
                            embed.AddField("Total Reps: " + reps, emoji_string);
                            await ctx.RespondAsync(embed);
                        }
                    }
                    else
                    {
                        await ctx.TriggerTypingAsync();
                        await ctx.RespondAsync(ctx.Message.Author.Mention + " slowdown phaggot.");
                    }
                }
                else
                {
                    await ctx.TriggerTypingAsync();
                    await ctx.RespondAsync(ctx.Message.Author.Mention + " slowdown phaggot.");
                    if (ProcessReps.RandomNumber(1, 10) == 1)
                    {
                        string reps = ProcessReps.deductCredits(ctx.Message.Author.Id.ToString(), "1");
                        string target = ctx.Message.Author.Mention;
                        await ctx.TriggerTypingAsync();

                        var green_role = ctx.Guild.Roles.Values.FirstOrDefault(x => x.Name == "green");
                        var red_role = ctx.Guild.Roles.Values.FirstOrDefault(x => x.Name == "red");
                        var emoji = DiscordEmoji.FromName(ctx.Client, ":rep:");
                        var emoji_string = "";
                        if (Convert.ToInt32(reps) > 0)
                        {
                            for (int i = 0; i < Convert.ToInt32(reps); i++)
                            {
                                if (i < 10)
                                    emoji_string += emoji;
                            }
                        }
                        else if (Convert.ToInt32(reps) < 0)
                        {
                            int counter = Convert.ToInt32(reps) * -1;
                            emoji = DiscordEmoji.FromName(ctx.Client, ":neg:");
                            for (int i = 0; i < counter; i++)
                            {
                                if (i < 10)
                                    emoji_string += emoji;
                            }
                            //emoji_string = emoji + emoji + emoji + emoji;
                        }
                        var newColor = new DiscordColor(DiscordColor.Green.Value);
                        if (Convert.ToInt32(reps) < 0)
                        {
                            newColor = new DiscordColor(DiscordColor.Red.Value);
                            await member.GrantRoleAsync(red_role);
                            await member.RevokeRoleAsync(green_role);
                        }
                        else
                        {
                            newColor = new DiscordColor(DiscordColor.Green.Value);
                            await member.GrantRoleAsync(green_role);
                            await member.RevokeRoleAsync(red_role);
                        }
                        string descriptio = "Has negged " + target;
                        var embed = new DiscordEmbedBuilder
                        {
                            Color = newColor,
                            Description = descriptio,
                            Author = new DiscordEmbedBuilder.EmbedAuthor
                            {
                                Name = ctx.Client.CurrentUser.Username,
                                IconUrl = ctx.Client.CurrentUser.AvatarUrl
                            },
                        };
                        embed.AddField("Total Reps: " + reps, emoji_string);
                        await ctx.RespondAsync(embed);
                    }
                }
            }
            else if(ctx.Message.Author.Id == member.Id)
            {
                await ctx.TriggerTypingAsync();
                await ctx.RespondAsync(ctx.Message.Author.Mention + " nope.");
            }
            else
            {
                if (Convert.ToInt32(ProcessReps.balance(ctx.Message.Author.Id.ToString())) < 0)
                {
                    if (ProcessReps.RandomNumber(1, 25) == 1)
                    {
                        await ctx.RespondAsync(ctx.Message.Author.Mention + " ( ☞ ͡° ͜ʖ ͡° ) ☞ ｄｏｎｔ　ｔａｌｋｉｎｇ　ｐｌｅａｓｅ ( ☞ ͡° ͜ʖ ͡° ) ☞");
                        cache.Add(ctx.Message.Author.Id.ToString(), ctx.Message.Author.Username.ToString(), new CacheItemPolicy()
                        { AbsoluteExpiration = DateTime.UtcNow.AddMinutes(15) });
                        return;
                    }
                }
                cache.Add(ctx.Message.Author.Id.ToString(), ctx.Message.Author.Username.ToString(), new CacheItemPolicy()
                { AbsoluteExpiration = DateTime.UtcNow.AddMinutes(15) });
                string reps = "";
                if (Convert.ToInt32(ProcessReps.balance(ctx.Message.Author.Id.ToString())) > 100)
                {
                    reps = ProcessReps.deductCredits(member.Id.ToString(), "2");
                }
                else
                {
                    reps = ProcessReps.deductCredits(member.Id.ToString(), "1");
                }
                string target = member.Mention;
                await ctx.TriggerTypingAsync();
                var green_role = ctx.Guild.Roles.Values.FirstOrDefault(x => x.Name == "green");
                var red_role = ctx.Guild.Roles.Values.FirstOrDefault(x => x.Name == "red");
                var emoji = DiscordEmoji.FromName(ctx.Client, ":rep:");
                var emoji_string = "";
                if (Convert.ToInt32(reps) > 0)
                {
                    for (int i = 0; i < Convert.ToInt32(reps); i++)
                    {
                        if (i < 10)
                            emoji_string += emoji;
                    }
                }
                else if (Convert.ToInt32(reps) < 0)
                {
                    int counter = Convert.ToInt32(reps) * -1;
                    emoji = DiscordEmoji.FromName(ctx.Client, ":neg:");
                    for (int i = 0; i < counter; i++)
                    {
                        if (i < 10)
                            emoji_string += emoji;
                    }
                    //emoji_string = emoji + emoji + emoji + emoji;
                }
                else
                {
                    emoji_string += emoji;
                }
                var newColor = new DiscordColor(DiscordColor.Green.Value);
                if (Convert.ToInt32(reps) < 0)
                {
                    newColor = new DiscordColor(DiscordColor.Red.Value);
                    await member.GrantRoleAsync(red_role);
                    await member.RevokeRoleAsync(green_role);
                }
                else
                {
                    newColor = new DiscordColor(DiscordColor.Green.Value);
                    await member.GrantRoleAsync(green_role);
                    await member.RevokeRoleAsync(red_role);
                }
                string descriptio = "Has negged " + target;
                if (ProcessReps.RandomNumber(1, 100) == 69)
                {
                    descriptio = "Has pegged " + target;
                    reps = ProcessReps.deductCredits(member.Id.ToString(), "2");

                }
                if (reason != null)
                {
                    descriptio = descriptio + " Reason: " + reason;
                }
                var embed = new DiscordEmbedBuilder
                {
                    Color = newColor,
                    Description = descriptio,
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = ctx.Message.Author.Username,
                        IconUrl = ctx.Message.Author.AvatarUrl
                    },
                };
                embed.AddField("Total Reps: " + reps, emoji_string);
                await ctx.RespondAsync(embed);
            }
        }
    }
}
