/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Protocols;
using TheXDS.MCART;
using TheXDS.MCART.Math;

namespace TheXDS.Proteus.Daemons
{
    public class TestDaemon : Daemon
    {
        private static readonly string[] _messages = {
            "There's a difference between something that's oldschool and something that's outdated.",
            "Oldschool is like the Atari 2600. Sure, the games are primitive, but they're still fun to play, you can always go back to them.",
            "Outdated, is something you never want to go back to.",
            "Tiger games are so outdated, they were never \"indated\". They were a fad, like pox.",
            "If they were an experiment in the 70's and only made a few of them, then I could accept that.",
            "But no! They melt these things for all that they're worth!",
            "You tought that LGN was the grand champion of the all-mighty shitty video game factory? Well, Tiger, put LGN into shame.",
            "Yeah, LGN laid down turd after turd after turd, but Tiger was like a machine-gun-ass, shooting down turd-turd-turd-turd-turd-turd-turd!",
            "These are the worst games I've ever had the honor of playing, even if you count them as videogames.",
            "People have discussions like \"are video games art\", or something like that... Well, I have a better one: Are Tiger games video games?",
            "These are a caveman's version of a video game, these were a step back in human evolution, these are the most desperate attempt of entertainment",
            "You can save up for a gameboy, or just go EHK-EHK-EHK-EHK-EHK-EHK-EHK...",
            "YEAH, WELL, EHK-EHK-EHK-EHK-EHK-EHK-EHK-EHK-EHK-EHK-EHK-EHK- just...",
            "WHAT THE HELL? These-thin-... HOW DID THEY WASTE SO MUCH PLASTIC TO MAKE THESE THINGS!?",
            "THEY BROUGHT THE GAME INDUSTRY DOWN AS LOW AS IT COULD GO!",
            "IT'S PROOF THAT JESUS DIED IN VAIN AND LEGALLY CHANGED HIS MIDDLE NAME TO FUCKING!",
            "THE ONLY THING I CAN THINK TO USE THESE THINGS FOR IS TO WIPE YOUR ASS WITH IT!!!",
            "... You might as well save that toilet paper. It's worth a whole lot more."
        };

        private int _i = 0;

        public override void Run()
        {
            ProteusLib.MessageTarget?.Show(this.NameOf(), _messages[_i]);
            Program.ServerOf<SessionProtocol>().SendAlert(_messages[_i]);
            _i = (++_i).Wrap(0, _messages.Length - 1);
        }
    }
}