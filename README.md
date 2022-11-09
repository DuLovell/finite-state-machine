# Finite State Machine

Реализация машины состояний с выделенными transitions и conditions, а так же с возможностью менять состояние по внешнему событию.

## Пример использования для обычной кнопки (не UnityEngine.UI)
### Машина состояний
```c#
    public class ButtonStateMachine : StateMachine
    {
        private ButtonAnimator _buttonAnimator = null!;

        private ButtonPushState _pushState = null!;
        private ButtonUnpushState _unpushState = null!;

        private void Awake()
        {
            _buttonAnimator = GetComponent<ButtonAnimator>();

            ConfigureStates();
			
            AddTransition(_unpushState, _pushState, InputReader.Instance.OnTouchBegan);
            AddTransition(_pushState, _unpushState, InputReader.Instance.OnTouchEnded);
        }

        private void Start()
        {
            SetState(_unpushState); // Переходим в дефолтное состояние
        }

        private void ConfigureStates()
        {
            _pushState = new ButtonPushState(_buttonAnimator);
            _unpushState = new ButtonUnpushState(_buttonAnimator);
        }
    } 
```
### Состояния
```c#
    public class ButtonUnpushState : State
    {
        private readonly ButtonAnimator _buttonAnimator;

        public MainButtonUnpushState(ButtonAnimator buttonAnimator)
        {
            _buttonAnimator = buttonAnimator;
        }
        
        public override void OnEnter()
        {
            _buttonAnimator.PlayUnpush();
        }

        public override void OnExit()
        {
        }

        public override string Name
        {
            get { return "Unpush"; }
        }
    }
```

```c#
    public class ButtonPushState : State
    {
        private readonly ButtonAnimator _buttonAnimator;

        public MainButtonPushState(ButtonAnimator buttonAnimator)
        {
            _buttonAnimator = buttonAnimator;
        }

        public override void OnEnter()
        {
            _buttonAnimator.PlayPush();
        }

        public override void OnExit()
        {
        }

        public override string Name
        {
            get { return "Push"; }
        }
    }
```
### Пример использования внешнего события с помощью ```EventObject```
```c#
    public class InputReader : Singleton<InputReader>
    {
        public readonly EventObject OnTouchBegan = new();
        public readonly EventObject OnTouchEnded = new();

        private void Update()
        {
            if (Input.touches.Length == 0) {
                return;
            }

            TouchPhase touchPhase = Input.GetTouch(0).phase;
            switch (touchPhase)
            {
                case TouchPhase.Began:
                    OnTouchBegan.Invoke();
                    break;
                case TouchPhase.Canceled or TouchPhase.Ended:
                    OnTouchEnded.Invoke();
                    break;
            }
        }
    }
```


## Ответы на вопросы
* Зачем нужен EventObject?

```EventObject``` нужен, чтобы машина состояний могла менять состояние по событию. Так как ```event Action``` это обычный делегат, иначе говоря, группа методов, то передать его в другой класс и подписаться на него не выйдет. Подробнее тут: https://stackoverflow.com/questions/8407886/pass-an-event-as-a-parameter-to-a-method

Поэтому в данном случае мы передаем ```EventObject```, который является полноценным классом, поэтому мы можем свободно передавать объект этого класса между классами и подписываться на его событие. Он содержит в себе событие ```Event```, на которое можно подписаться и метод ```Invoke()```, который это событие вызывает.

* Зачем нужно свойство Name в состояниях?

В классе ```StateMachine``` есть поле ```_currentStateName```, которое нужно только чтобы показывать текущее состояние в инспекторе. Соответственно, чтобы в инспекторе отображалось удобно читаемое название состояния, а не полное название класса, была добавлена возможность называть его самостоятельно в отдельном свойстве.
