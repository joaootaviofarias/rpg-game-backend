﻿using RPGGame.Config;
using RPGGame.Game.Commands;
using RPGGame.Infrastructure;
using SixLabors.ImageSharp;

namespace RPGGame.Game
{
    public class Game : IGame
    {
        private readonly CommandQueue _commands;
        private NpcCommadQueue _npcCommands;
        private Person Hero, Npc;
        private Map Map;
        private IEnumerable<ObjectToProcess> _collisionObjects;
        private object State;


        public Game(CommandQueue commands)
        {
            _commands = commands;
        }

        public void Init()
        {
            Map = new Map("Map", @"Assets\maps\DemoLower.png", 192, 192, 16, 16);
            Map.Collision.AddCollisionBody(1, 3);
            Map.Collision.AddCollisionBody(2, 3);
            Map.Collision.AddCollisionBody(3, 3);
            Map.Collision.AddCollisionBody(4, 3);
            Map.Collision.AddCollisionBody(5, 3);
            Map.Collision.AddCollisionBody(6, 4); 
            Map.Collision.AddCollisionBody(7, 3);
            Map.Collision.AddCollisionBody(8, 4);
            Map.Collision.AddCollisionBody(9, 3);
            Map.Collision.AddCollisionBody(10, 3);
            Map.Collision.AddCollisionBody(11, 4);
            Map.Collision.AddCollisionBody(11, 5);
            Map.Collision.AddCollisionBody(11, 6);
            Map.Collision.AddCollisionBody(11, 7);
            Map.Collision.AddCollisionBody(11, 8);
            Map.Collision.AddCollisionBody(11, 9);
            Map.Collision.AddCollisionBody(10, 10);
            Map.Collision.AddCollisionBody(9, 10);
            Map.Collision.AddCollisionBody(8, 10);
            Map.Collision.AddCollisionBody(7, 10);
            Map.Collision.AddCollisionBody(6, 10);
            Map.Collision.AddCollisionBody(5, 11);
            Map.Collision.AddCollisionBody(4, 10);
            Map.Collision.AddCollisionBody(3, 10);
            Map.Collision.AddCollisionBody(2, 10);
            Map.Collision.AddCollisionBody(1, 10);
            Map.Collision.AddCollisionBody(0, 9);
            Map.Collision.AddCollisionBody(0, 8);
            Map.Collision.AddCollisionBody(0, 7);
            Map.Collision.AddCollisionBody(0, 6);
            Map.Collision.AddCollisionBody(0, 5);
            Map.Collision.AddCollisionBody(0, 4);
            Map.Collision.AddCollisionBody(0, 3);
            Map.Collision.AddCollisionBody(7, 6);
            Map.Collision.AddCollisionBody(8, 6);
            Map.Collision.AddCollisionBody(7, 7);
            Map.Collision.AddCollisionBody(8, 7);

            Hero = new Person("Hero", @"Assets\characters\people\hero.png", 6, 7, 128, 128, 32, 32);
            Hero.Main = true;
            Hero.Sprite.Animation = new Animation()
                .AddAnimation("IdleUp", new List<Point> { new Point(0, 2) })
                .AddAnimation("IdleDown", new List<Point> { new Point(0, 0) })
                .AddAnimation("IdleLeft", new List<Point> { new Point(0, 3) })
                .AddAnimation("IdleRight", new List<Point> { new Point(0, 1) })
                .AddAnimation("WalkUp", new List<Point> { new Point(0, 2), new Point(1, 2), new Point(2, 2), new Point(3, 2) })
                .AddAnimation("WalkDown", new List<Point> { new Point(0, 0), new Point(1, 0), new Point(2, 0), new Point(3, 0) })
                .AddAnimation("WalkLeft", new List<Point> { new Point(0, 3), new Point(1, 3), new Point(2, 3), new Point(3, 3) })
                .AddAnimation("WalkRight", new List<Point> { new Point(0, 1), new Point(1, 1), new Point(2, 1), new Point(3, 1) });

            Npc = new Person("Npc", @"Assets\characters\people\npc1.png", 7, 9, 128, 128, 32, 32);
            Npc.Sprite.Animation = new Animation()
                .AddAnimation("IdleUp", new List<Point> { new Point(0, 2) })
                .AddAnimation("IdleDown", new List<Point> { new Point(0, 0) })
                .AddAnimation("IdleLeft", new List<Point> { new Point(0, 3) })
                .AddAnimation("IdleRight", new List<Point> { new Point(0, 1) })
                .AddAnimation("WalkUp", new List<Point> { new Point(0, 2), new Point(1, 2), new Point(2, 2), new Point(3, 2) })
                .AddAnimation("WalkDown", new List<Point> { new Point(0, 0), new Point(1, 0), new Point(2, 0), new Point(3, 0) })
                .AddAnimation("WalkLeft", new List<Point> { new Point(0, 3), new Point(1, 3), new Point(2, 3), new Point(3, 3) })
                .AddAnimation("WalkRight", new List<Point> { new Point(0, 1), new Point(1, 1), new Point(2, 1), new Point(3, 1) });

            _npcCommands = new NpcCommadQueue(new List<Key> { Key.Default });
        }

        public void Update(double deltaTime)
        {
            var objectsToProcess = new List<ObjectToProcess>()
            {
                new ObjectToProcess(Hero, _commands.CurrentKey, () => _commands.GetKey()),
                new ObjectToProcess(Npc, (Key)_npcCommands.Current, () => _npcCommands.MoveNext()),
                new ObjectToProcess(Map, Key.Default, () => { })
            };

            CollisionProcessor.Process(objectsToProcess);
            CommandProcessor.Process(objectsToProcess);
            Camera.SetPositions(objectsToProcess);

            State = new
            {
                GameObjects = CreateGameObjetcs(Map, Npc, Hero),
                CommandsPressed = _commands.KeysPressed.Count()
            };
        }

        private List<GameObjectDto> CreateGameObjetcs(params IGameObject[] gameObjects)
        {
            var gameObjectsDto = new List<GameObjectDto>();
            foreach (var gameObject in gameObjects)
            {
                var gameObjectDto = new GameObjectDto
                {
                    Name = gameObject.Name,
                    Sprite = gameObject.Sprite.Image,
                    X = gameObject.RelativeX,
                    Y = gameObject.RelativeY,
                    MinX = gameObject.MinX,
                    MinY = gameObject.MinY,
                    MaxX = gameObject.MaxX,
                    MaxY = gameObject.MaxY,
                    CollisionBodies = gameObject.Collision.CollisionBodies.Select(c => new CollisionBodyDto
                    {
                        Id = c.Id,
                        HasCollision = c.HasCollision,
                        RelativeX = c.RelativeX,
                        RelativeY = c.RelativeY,
                    }).ToList()
                };

                gameObjectsDto.Add(gameObjectDto);
            }

            return gameObjectsDto;
        }

        public object GetState()
        {
            return State;
        }
    }
}
