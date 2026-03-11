using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MyGame
{
    public class GamePresenter : ITickable
    {
        readonly HelloWorldService helloWorldService;

        public GamePresenter(HelloWorldService helloWorldService)
        {
            this.helloWorldService = helloWorldService;
        }

        void ITickable.Tick()
        {
            // helloWorldService.Hello();
        }
    }
}
