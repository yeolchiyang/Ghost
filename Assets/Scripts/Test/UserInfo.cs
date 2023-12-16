using RSG;
using UnityEngine;

public class UserInfo : MonoBehaviour
{
    /// <summary>
    /// Goal to move towards
    /// </summary>
    public Transform goal;

    /// <summary>
    /// State machine
    /// </summary>
    private IState rootState;

    /// <summary>
    /// Distance to retreat before approaching the target again.
    /// </summary>
    float resetDistance = 10f;

    private class MovingState : AbstractState
    {
        public float movementSpeed = 3f;
    }

    private class RetreatingState : MovingState
    {
        public Vector3 direction;
    }

    // Use this for initialization
    void Start()
    {
        rootState = new StateMachineBuilder()
            /* "Approach" State정의 시작
             * State<MovingState>("Approach") 생성자는 아래의 생성자로 만든
             * new StateBuilder를 반환
             * 
             * return new StateBuilder<MovingState, StateMachineBuilder>
             * (this(==Statemachinebuilder), rootState, "Approach");
             * StateBuilder(TParent parentBuilder, AbstractState parentState, string name)
             * 생성자는 아래의 맴버변수들을 채움
             * this.parentBuilder = parentBuilder; (this.parentBuilder = Statemachinebuilder)
             * state(StateBuilder멤버변수) = new T(); (state = new MovingState())
             * parentState.AddChild(state, name); (parentState.AddChild(state,"Approach"))
             * parentState.AddChild(state, name) 는 
             * 요약 : State<T>("string") 실행 시
             * new T()로 생성된 객체가, parentState(=rootState).AddChild(state, name); 에 의해
             * children Dictionary에 추가됨
             * rootState는 newState(new T()로 생성된 객체).Parent 로 등록됨
             * 
             */
            .State<MovingState>("Approach")
                /* StateBuilder.Enter(Action<T> onEnter) 메서드는
                 * this.state.SetEnterAction(onEnter(state)); 를 실행하고
                 * return this; 함
                 * onEnter에는 항상
                 * AbstractState를 상속받은 클래스로 만든 매개변수 넣어야하도록 제네릭 지정
                 * 
                 * State.SetEnterAction(Action onEnter) 메서드는
                 * this.onEnter = onEnter; 
                 * (State.onEneter 메소드를, Action onEnter 역할하도록 함)
                 * 아래에서는 onEnter(state)를 하면 Debug.Log("Entering Approach state"); 출력하도록 되어있음
                 * 요약 : 위에서 State로 생성된 객체 안의, onEnter 함수 할당
                 */
                .Enter(state =>
                {
                    Debug.Log("Entering Approach state");
                })
                /* StateBuilder.Update(Action<T, float> onUpdate) 메서드는
                 * state.SetUpdateAction(dt => onUpdate(state, dt)); 를 실행하고
                 * return this; 함
                 * onUpdate에는 항상
                 * AbstractState를 상속받은 클래스로 만든 매개변수,
                 * float 타입의 매개변수를 가진 함수를 넣어야만 하도록 제네릭으로 지정됨
                 * 
                 * State.SetUpdateAction(Action<float> onUpdate) 메서드는
                 * this.onUpdate = onUpdate; 매개변수 onUpdate 함수를 state.onUpdate에 할당
                 * Update(Action<T, float> onUpdate) 요약 : 
                 * onUpdate 함수 내용 할당
                 */
                .Update((state, deltaTime) =>
                {
                    var directionToTarget = transform.position - goal.position;
                    directionToTarget.y = 0; // Only calculate movement on a 2d plane
                    directionToTarget.Normalize();

                    transform.position -= directionToTarget * deltaTime * state.movementSpeed;
                })
                /* Event(string identifier, Action<T> action) 메서드는
                 * state.SetEvent<EventArgs>(identifier, _ => action(state)); 를 실행하고
                 * return this; 함
                 * action에 들어갈 메소드는 항상
                 * AbstractState를 상속받은 클래스로 만든 매개변수를 넣어야만 하도록 제네릭으로 지정됨
                 * State.SetEvent<TEvent>(string identifier, Action<TEvent> eventTriggeredAction) 메서드는
                 * events.Add(identifier, args => eventTriggeredAction(CheckEventArgs<TEvent>(identifier, args)));
                 * events 딕셔너리에, 
                 * Key : identifier 
                 * Value : args => eventTriggeredAction(CheckEventArgs<TEvent>(identifier, args)) 
                 * 추가
                 * TEvent CheckEventArgs<TEvent>(string identifier, EventArgs args) 
                 * 이건 그냥 args 반환함
                 * Value에는 항상 EventArgs 를 상속받은 매개변수(args)를 가진 
                 * eventTriggeredAction 함수가 들어감
                 * 
                 * 결론
                 * events 딕셔너리에
                 * Key : identifier 
                 * Value : action
                 * 추가하는 함수
                 * 
                 * 아래 이벤트 함수(action) 내용은
                 * state.PushState("Retreat");로
                 * children에서 "Retreat" key로 가져온 state를
                 * activeChildren에 넣음
                 * 
                 * Event(string identifier, Action<T> action) 요약: 
                 * events Dictionary에, 
                 * key : identifier , Value : action 추가
                 */
                .Event("TargetReached", state =>
                {
                    state.PushState("Retreat");
                })
                /* Exit(Action<T> onExit)
                 * state.SetExitAction(() => onExit(state));
                 * return this; 하는 함수
                 * state.SetExitAction(() => onExit(state))
                 * this.onExit = onExit;
                 * Exit(Action<T> onExit); 요약
                 * state.onExit = onExit; 하는 함수
                 */
                .Exit(state =>
                {
                    Debug.Log("Exiting Approach state");
                })
                /* parentBuilder = StateBuilder
                 * return new StateBuilder<State, IStateBuilder<T, TParent>>(this, state, name);
                 * this = StateBuilder
                 * state = "Approach"
                 * name = "Retreat"
                 * 
                 * StateBuilder(TParent parentBuilder, AbstractState parentState, string name)
                 * this.parentBuilder = parentBuilder(=StateBuilder);
                 * state = new T();
                 * 
                 * parentState.AddChild(state, name); (parentState = parentBuilder(=StateBuilder) 멤버변수)
                 * parentState.children.Add(stateName, newState);
                 * newState( = state = new T()).Parent = parentState;
                 * "Retreat" 로 지정된 State의 parentState는 "Approach"
                 */
                .State<RetreatingState>("Retreat")
                    // Set a new destination
                    .Enter(state =>
                    {
                        Debug.Log("Entering Retreat state");

                        // Work out a new target, away from the goal
                        var direction = new Vector3(Random.value, 0f, Random.value);
                        direction.Normalize();

                        state.direction = direction;
                    })
                    // Move towards the new destination
                    .Update((state, deltaTime) =>
                    {
                        transform.position -= state.direction * deltaTime * state.movementSpeed;
                    })
                    // If we go further away from the original target than the reset distance, exit and 
                    // go back to the previous state
                    /* Condition(Func<bool> predicate, Action<T> action)
                     * state.SetCondition(predicate, () => action(state));
                     * return this; 하는 함수
                     * 
                     * SetCondition(Func<bool> predicate, Action action)
                     * conditions.Add(new Condition() {
                     *      Predicate = predicate,
                     *      Action = action
                     * });
                     */
                    //private struct Condition
                    //{
                    //    public Func<bool> Predicate;
                    //    public Action Action;
                    //}
                    /*
                     * conditions 리스트에 Condition 구조체 객체 추가
                     * Predicate 로 정의된 멤버함수(반환값 bool)
                     * Action 으로 정의된 멤버함수(반환값 없음)
                     * 이곳에서 정의된 함수들은, state.Update 함수 실행 시
                     * 주기적으로 실행 됨
                     * Predicate 함수가 먼저 실행되어, 조건을 체크,
                     * 조건을 만족하면 Action 실행
                     * Condition(Func<bool> predicate, Action<T> action) 요약 : 
                     * conditions 리스트에 Condition 구조체 객체 추가
                     * 
                     */
                    .Condition(() =>
                    {
                        return Vector3.Distance(transform.position, goal.position) >= resetDistance;
                    },
                    state =>
                    {
                        /* activeChildren 맨위요소 반환 후
                         * 그 요소.PopState() 실행 후
                         * onExit(); 실행하는 함수임
                         * state는 StateBuilder의 멤버변수
                         * state.Parent = "Retreat" 임
                         * Retreat의 activeChildren.Pop.Exit() 함
                         * 상위 state의 activeChildren 삭제
                         */
                        state.Parent.PopState();

                        //상위 state의 Enter 실행
                    })
                    .Exit(state =>
                    {
                        Debug.Log("Exiting Retreat state");
                    })
                    .End()//StateBuilder().parentBuilder 반환(State<MovingState>("Approach") 에서 생성된 StateBuilder)
                .End()//StateBuilder().parentBuilder 반환(제일 상위인 StateMachineBuilder)
            .Build();//rootState 반환

        /* children 딕셔너리에 "Approach" key 로 등록한 MovingState 반환 받고,
         * 현재 실행중인 activeChildren 빼내고
         * activeChildren.Push(newState); 한 뒤,
         * newState.Enter함수 실행
         */
        rootState.ChangeState("Approach");
    }

    // Update is called once per frame
    void Update()
    {
        /* activeChildren의 onUpdate 함수 실행
         * 까보면 최종적으로 activeChildren.Peek().Update(Time.deltaTime); 임
         */
        rootState.Update(Time.deltaTime);
    }

    /// <summary>
    /// Tell our state machine that the target has been reached once we hit the trigger
    /// </summary>
    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform == goal)
        {
            /* TriggerEvent(string name) 실행되면
             * TriggerEvent(name, EventArgs.Empty); 실행되고
             * activeChildren.Peek().TriggerEvent(name, eventArgs); 실행됨
             * activeChildren.activeChildren.Count == 0 일 것이기에
             * events 딕셔너리에서, "TargetReached" 로 설정한 Action 메소드를 가져와
             * 해당 메소드 실행
             * 아래 것이 실행되면,
             * state.PushState("Retreat"); 가 실행됨
             */
            rootState.TriggerEvent("TargetReached");
        }
    }


}
