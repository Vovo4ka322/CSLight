﻿using System;
using System.Collections.Generic;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Battle battle = new Battle();

            battle.Work();
        }
    }
    class Squad
    {  
        public Squad(List<Fighter> fighters)
        {
            Fighters = new List<Fighter>(fighters);
        }

        public List<Fighter> Fighters { get; private set; }

        public void ShowInfo()
        {
            foreach (var fighter in Fighters)
            {
                fighter.ShowInfo();
            }
        }
    }

    class SquadFactory
    {
        public Squad CreateSquad()
        {
            return new Squad(CreateFighters());
        }

        private List<Fighter> CreateFighters()
        {
            return new List<Fighter>()
                {
                new Tank("Танк", 1000, 100, 2),
                new WarPlane("Истребитель", 500, 200, 1),
                new Warship("Корабль", 1500, 150, 2),
                new Infantry("Пехота", 300, 50, 4),
                new Artillery("Артиллерия", 500, 250, 1)
                };
        }
    }

    class Fighter
    {
        public Fighter(string type, int health, int damage, int attemptUseSpell)
        {
            Type = type;
            Health = health;
            Damage = damage;
            AttemptUseSpell = attemptUseSpell;
        }

        public string Type { get; private set; }

        public int Damage { get; protected set; }

        public int Health { get; protected set; }

        public int AttemptUseSpell { get; protected set; }

        public bool Alive => Health > 0;

        public virtual void Attack(Fighter fighter)
        {
            fighter.TakeDamage(Damage);
        }

        public void ShowInfo()
        {
            Console.WriteLine($"Боец {Type} имеет {Health} здоровья и наносит {Damage} урона.");
        }

        private void TakeDamage(int damage)
        {
            Health -= damage;
        }

        public virtual void ShowSpell()
        {
            Console.WriteLine("Особая спрособность боевой единицы: ");
        }

        public virtual bool TryUseSpell(int minHealthThreshold, int damageBySpell, int numberOfAttempts)
        {
            if (Health <= minHealthThreshold && AttemptUseSpell >= numberOfAttempts)
            {
                AttemptUseSpell -= numberOfAttempts;

                Damage += damageBySpell;

                return true;
            }

            return false;
        }

        public virtual void UseSpell() { }
    }

    class Tank : Fighter
    {
        private int _minHealthThreshold = 800;
        private int _damageBySpell = 200;
        private int _numberOfAttempts = 1;
        private int _baseDamage;

        public Tank(string type, int health, int damage, int attemptUseSpell) : base(type, health, damage, attemptUseSpell)
        {
            _baseDamage = Damage;
        }

        public void UseShells()
        {
            Damage = _baseDamage;

            if (TryUseSpell(_minHealthThreshold, _damageBySpell, _numberOfAttempts))
            {
                Console.WriteLine($"Танк нанес урон {_damageBySpell} усиленным снарядом");
            }
        }

        public override void UseSpell()
        {
            UseShells();
        }

        public override void ShowSpell()
        {
            base.ShowSpell();

            Console.WriteLine($"Имеет {AttemptUseSpell} усиленных снаряда, наносящие урон в размере {_damageBySpell}");
        }
    }

    class WarPlane : Fighter
    {
        public WarPlane(string type, int health, int damage, int attemptUseSpell) : base(type, health, damage, attemptUseSpell) { }

        public override void Attack(Fighter fighter)
        {
            if (EvadeByDamage())
            {
                Console.WriteLine("Истребитель увернулся от атаки");
            }
            else
            {
                base.Attack(fighter);
            }
        }

        public bool EvadeByDamage()
        {
            int minValue = 1;
            int maxValue = 11;
            int hitChance = UserUtils.GenerateRandomNumber(minValue, maxValue);

            if (hitChance > 5)
            {
                return true;
            }

            return false;
        }

        public override void ShowSpell()
        {
            base.ShowSpell();

            Console.WriteLine("Имеет шанс уклонения от вражеской атаки");
        }
    }

    class Warship : Fighter
    {
        public Warship(string type, int health, int damage, int attemptUseSpell) : base(type, health, damage, attemptUseSpell) { }
    }

    class Infantry : Fighter
    {
        public Infantry(string type, int health, int damage, int attemptUseSpell) : base(type, health, damage, attemptUseSpell) { }
    }

    class Artillery : Fighter
    {
        private int _minHealthThreshold = 500;
        private int _damageBySpell = 400;
        private int _numberOfAttempts = 1;
        private int _baseDamage;

        public Artillery(string type, int health, int damage, int attemptUseSpell) : base(type, health, damage, attemptUseSpell)
        {
            _baseDamage = Damage;
        }

        public void UseShells()
        {
            Damage = _baseDamage;

            if (TryUseSpell(_minHealthThreshold, _damageBySpell, _numberOfAttempts))
            {
                Console.WriteLine($"Артиллерийское орудие нанесл урон {_damageBySpell} усиленным снарядом");
            }
        }

        public override void UseSpell()
        {
            UseShells();
        }

        public override void ShowSpell()
        {
            base.ShowSpell();

            Console.WriteLine($"Имеет {AttemptUseSpell} снаряд, наносящие урон в размере {_damageBySpell}");
        }
    }

    class Battle
    {
        private Squad _firstSquad;
        private Squad _secondSquad;

        public void Work()
        {
            SquadFactory squadFactory = new SquadFactory();
            _firstSquad = squadFactory.CreateSquad();
            _secondSquad = squadFactory.CreateSquad();

            Console.Write("1) ");

            Console.Write("2) ");

            int minValue = 1;

            for (int i = 0; i < _firstSquad.Fighters.Count && i < _secondSquad.Fighters.Count; i++)
            {
                int randomIndex = UserUtils.GenerateRandomNumber(minValue, _secondSquad.Fighters.Count);

                while (_firstSquad.Fighters[i].Health > 0 && _secondSquad.Fighters[randomIndex].Health > 0)
                {
                    _firstSquad.Fighters[i].Attack(_secondSquad.Fighters[randomIndex]);
                    _secondSquad.Fighters[randomIndex].Attack(_firstSquad.Fighters[i]);

                    _firstSquad.Fighters[i].UseSpell();
                    _secondSquad.Fighters[randomIndex].UseSpell();

                    Console.Write("1) ");

                    _firstSquad.Fighters[i].ShowInfo();

                    Console.Write("2) ");

                    _secondSquad.Fighters[randomIndex].ShowInfo();
                }

                if (_firstSquad.Fighters[i].Health <= 0)
                {
                    _firstSquad.Fighters.RemoveAt(i);
                }
                else if (_secondSquad.Fighters[i].Health <= 0)
                {
                    _secondSquad.Fighters.RemoveAt(i);
                }
            }

            DetermineWinner(_firstSquad, _secondSquad);
        }

        private void DetermineWinner(Squad firstSquad, Squad secondSquad)
        {
            if (firstSquad.Fighters.Count == secondSquad.Fighters.Count)
            {
                Console.WriteLine("Ничья, победила дружба!");
            }
            else if (firstSquad.Fighters.Count > secondSquad.Fighters.Count)
            {
                Console.WriteLine("Победил первый отряд!");
            }
            else if (secondSquad.Fighters.Count > firstSquad.Fighters.Count)
            {
                Console.WriteLine("Победил второй отряд!");
            }
        }
    }

    class UserUtils
    {
        private static Random _random = new Random();

        public static int GenerateRandomNumber(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue);
        }
    }
}