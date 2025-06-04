// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AdvancedCommandLibrary
//    Project:          AdvancedCommandLibrary
//    FileName:         TestCommand.cs
//    Author:           Redforce04#4091
//    Revision Date:    05/26/2025 15:13
//    Created Date:     05/26/2025 15:05
// -----------------------------------------

/*
namespace AdvancedCommandLibrary.Testing;

using System.Collections.Generic;
using System.Net;
using Attributes;
using Contexts;
using LabApi.Features.Wrappers;

public class TestCommand
{
    public static List<Vote> PastVotes { get; set; }
    public static Vote? OngoingVote { get; set; }
    
    [Command("callvote", "Calls a vote via broadcast.", [ "cv" ])]
    [RequirePermissions("VotingSystem.Callvote")]
    public static void CallVote(CommandContext context)
    {
        if (!context.CheckPermissions())
            return;
        if(OngoingVote is not null)
            context.Deny("A vote is already running.");
    }

    [Command("vote", "Votes on poll", commandHandlerType: CommandHandlerType.ClientConsole)]
    public static void VoteC(CommandContext context)
    {
        if (OngoingVote is null)
        {
            context.Deny("No vote is running.");
            return;
        }
        // OngoingVote.Value.
    }

    public struct Vote
    {
        public bool Ongoing { get; set; }

        public int VotesFor => this.playersFor.Count;
        public int VotesNay => this.playersNay.Count;
        public int VotesUndecided => Player.List.Count - (playersFor.Count + playersNay.Count);
        
        private List<Player> playersFor = new();
        private List<Player> playersNay = new();

        public Vote()
        {
            Ongoing = false;
        }

        internal bool AddForVote(Player player)
        {
            if (playersFor.Contains(player))
                return false;
            if(playersNay.Contains(player))
                return false;
            playersFor.Add(player);
            return true;
        }

        internal bool AddNayVote(Player player)
        {
            if (playersFor.Contains(player))
                return false;
            if(playersNay.Contains(player))
                return false;
            
            playersNay.Add(player);
            return true;
        }
    }
}
*/