using System;
using System.Collections.Generic;
using System.Linq;

public enum Suit
{
    Hearts, Diamonds, Clubs, Spades
}

public enum Rank
{
    Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace
}

public class Card
{
    public string Suit { get; }
    public string Rank { get; }

    public Card(string suit, string rank)
    {
        Suit = suit;
        Rank = rank;
    }

    public int GetValue()
    {
        switch (Rank)
        {
            case "Ace":
                return 11;
            case "2":
                return 2;
            case "3":
                return 3;
            case "4":
                return 4;
            case "5":
                return 5;
            case "6":
                return 6;
            case "7":
                return 7;
            case "8":
                return 8;
            case "9":
                return 9;
            case "10":
            case "Jack":
            case "Queen":
            case "King":
                return 10;
            default:
                return 0;
        }
    }

    public override string ToString()
    {
        return $"{Rank} of {Suit}";
    }
}

public class Deck
{
    private List<Card> _cards;
    private Random _random;

    public Deck()
    {
        _cards = new List<Card>();
        _random = new Random();
        string[] suits = { "Clubs", "Diamonds", "Hearts", "Spades" };
        string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King", "Ace" };
        foreach (var suit in suits)
        {
            foreach (var rank in ranks)
            {
                _cards.Add(new Card(suit, rank));
            }
        }

        Shuffle();
    }

    public void Shuffle()
    {
        for (int i = 0; i < _cards.Count; i++)
        {
            int j = _random.Next(i, _cards.Count);
            var temp = _cards[i];
            _cards[i] = _cards[j];
            _cards[j] = temp;
        }
    }

    public Card DrawCard()
    {
        if (_cards.Count == 0)
            throw new InvalidOperationException("The deck is empty!");

        var card = _cards[0];
        _cards.RemoveAt(0);
        return card;
    }
}
public abstract class Participant
{
    public List<Card> _hand { get; private set; }

    public Participant()
    {
        _hand = new List<Card>();
    }

    public void TakeCard(Card card)
    {
        _hand.Add(card);
    }

    public int GetHandValue()
    {
        int value = 0;
        int aceCount = 0;

        foreach (var card in _hand)
        {
            value += card.GetValue();
            if (card.Rank == "Ace")
            {
                aceCount++;
            }
        }

        while (value > 21 && aceCount > 0)
        {
            value -= 10;
            aceCount--;
        }

        return value;
    }

    public bool IsBusted => GetHandValue() > 21;

    public bool HasBlackjack => GetHandValue() == 21 && _hand.Count == 2;
}

public class Player : Participant
{
    public bool IsStanding { get; private set; } = false;

    public void Stand()
    {
        IsStanding = true;
    }
}

public class Dealer : Participant
{
    public void PlayTurn(Deck deck)
    {
        while (GetHandValue() < 17)
        {
            TakeCard(deck.DrawCard());
        }
    }
}

//public class GameAnalyzer
//{
//    private int _numberOfSimulations;
//    private int _playerWins;
//    private int _dealerWins;
//    private int _ties;

//    public GameAnalyzer(int numberOfSimulations)
//    {
//        _numberOfSimulations = numberOfSimulations;
//    }

//    public void Run()
//    {
//        for (int i = 0; i < _numberOfSimulations; i++)
//        {
//            var result = SimulateGame();
//            switch (result)
//            {
//                case GameResult.PlayerWin:
//                    _playerWins++;
//                    break;
//                case GameResult.DealerWin:
//                    _dealerWins++;
//                    break;
//                case GameResult.Tie:
//                    _ties++;
//                    break;
//            }
//        }

//        Console.WriteLine($"Games Simulated: {_numberOfSimulations}");
//        Console.WriteLine($"Player Wins: {_playerWins} ({(_playerWins / (float)_numberOfSimulations) * 100:F2}%)");
//        Console.WriteLine($"Dealer Wins: {_dealerWins} ({(_dealerWins / (float)_numberOfSimulations) * 100:F2}%)");
//        Console.WriteLine($"Ties: {_ties} ({(_ties / (float)_numberOfSimulations) * 100:F2}%)");
//    }

//    private GameResult SimulateGame()
//    {
//        var deck = new Deck();
//        var player = new Player();
//        var dealer = new Dealer();

//        // Начальная раздача
//        player.TakeCard(deck.DrawCard());
//        player.TakeCard(deck.DrawCard());
//        dealer.TakeCard(deck.DrawCard());
//        dealer.TakeCard(deck.DrawCard());

//        // Ход игрока (базовая стратегия: тянуть карту, если меньше 17, и стоять, если 17 или больше)
//        while (player.GetHandValue() < 17)
//        {
//            player.TakeCard(deck.DrawCard());
//        }

//        if (player.IsBusted)
//        {
//            return GameResult.DealerWin;
//        }

//        // Ход дилера
//        dealer.PlayTurn(deck);

//        if (dealer.IsBusted)
//        {
//            return GameResult.PlayerWin;
//        }

//        // Определение победителя
//        int playerHandValue = player.GetHandValue();
//        int dealerHandValue = dealer.GetHandValue();

//        if (playerHandValue > dealerHandValue)
//        {
//            return GameResult.PlayerWin;
//        }
//        else if (playerHandValue < dealerHandValue)
//        {
//            return GameResult.DealerWin;
//        }
//        else
//        {
//            return GameResult.Tie;
//        }
//    }

//    private enum GameResult
//    {
//        PlayerWin,
//        DealerWin,
//        Tie
//    }
//}
public class BlackjackGame
{
    private Deck _deck;
    private Player _player;
    private Dealer _dealer;

    public void StartGame()
    {
        _deck = new Deck();
        _player = new Player();
        _dealer = new Dealer();

        // Initial deal
        _player.TakeCard(_deck.DrawCard());
        _player.TakeCard(_deck.DrawCard());
        _dealer.TakeCard(_deck.DrawCard());
        _dealer.TakeCard(_deck.DrawCard());

        ShowGameState();

        while (!_player.IsStanding && !_player.IsBusted && !_player.HasBlackjack)
        {
            Console.WriteLine("Choose action: (1) Hit (2) Stand");
            string choice = Console.ReadLine();
            if (choice == "1")
            {
                _player.TakeCard(_deck.DrawCard());
            }
            else if (choice == "2")
            {
                _player.Stand();
            }

            ShowGameState();
        }

        if (_player.IsBusted)
        {
            Console.WriteLine("Player busted! Dealer wins. 😔");
            return;
        }

        _dealer.PlayTurn(_deck);
        ShowGameState(showAllDealerCards: true);

        if (_dealer.IsBusted || _player.GetHandValue() > _dealer.GetHandValue())
        {
            Console.WriteLine("Player wins! 🎉");
        }
        else if (_player.GetHandValue() == _dealer.GetHandValue())
        {
            Console.WriteLine("It's a tie! 🤝");
        }
        else
        {
            Console.WriteLine("Dealer wins! 😐");
        }
    }

    private void ShowGameState(bool showAllDealerCards = false)
    {
        Console.WriteLine("\nPlayer's Hand:");
        foreach (var card in _player._hand)
        {
            Console.WriteLine(card);
        }
        Console.WriteLine($"Hand Value: {_player.GetHandValue()}");

        Console.WriteLine("\nDealer's Hand:");
        if (showAllDealerCards)
        {
            foreach (var card in _dealer._hand)
            {
                Console.WriteLine(card);
            }
            Console.WriteLine($"Hand Value: {_dealer.GetHandValue()}");
        }
        else
        {
            Console.WriteLine(_dealer._hand.First());
            Console.WriteLine("Hidden Card");
        }
    }

    public static void Main(string[] args)
    {
        BlackjackGame game = new BlackjackGame();
        game.StartGame();
        Console.ReadKey();
    }
}
