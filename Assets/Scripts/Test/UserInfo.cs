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
            /* "Approach" State���� ����
             * State<MovingState>("Approach") �����ڴ� �Ʒ��� �����ڷ� ����
             * new StateBuilder�� ��ȯ
             * 
             * return new StateBuilder<MovingState, StateMachineBuilder>
             * (this(==Statemachinebuilder), rootState, "Approach");
             * StateBuilder(TParent parentBuilder, AbstractState parentState, string name)
             * �����ڴ� �Ʒ��� �ɹ��������� ä��
             * this.parentBuilder = parentBuilder; (this.parentBuilder = Statemachinebuilder)
             * state(StateBuilder�������) = new T(); (state = new MovingState())
             * parentState.AddChild(state, name); (parentState.AddChild(state,"Approach"))
             * parentState.AddChild(state, name) �� 
             * ��� : State<T>("string") ���� ��
             * new T()�� ������ ��ü��, parentState(=rootState).AddChild(state, name); �� ����
             * children Dictionary�� �߰���
             * rootState�� newState(new T()�� ������ ��ü).Parent �� ��ϵ�
             * 
             */
            .State<MovingState>("Approach")
                /* StateBuilder.Enter(Action<T> onEnter) �޼����
                 * this.state.SetEnterAction(onEnter(state)); �� �����ϰ�
                 * return this; ��
                 * onEnter���� �׻�
                 * AbstractState�� ��ӹ��� Ŭ������ ���� �Ű����� �־���ϵ��� ���׸� ����
                 * 
                 * State.SetEnterAction(Action onEnter) �޼����
                 * this.onEnter = onEnter; 
                 * (State.onEneter �޼ҵ带, Action onEnter �����ϵ��� ��)
                 * �Ʒ������� onEnter(state)�� �ϸ� Debug.Log("Entering Approach state"); ����ϵ��� �Ǿ�����
                 * ��� : ������ State�� ������ ��ü ����, onEnter �Լ� �Ҵ�
                 */
                .Enter(state =>
                {
                    Debug.Log("Entering Approach state");
                })
                /* StateBuilder.Update(Action<T, float> onUpdate) �޼����
                 * state.SetUpdateAction(dt => onUpdate(state, dt)); �� �����ϰ�
                 * return this; ��
                 * onUpdate���� �׻�
                 * AbstractState�� ��ӹ��� Ŭ������ ���� �Ű�����,
                 * float Ÿ���� �Ű������� ���� �Լ��� �־�߸� �ϵ��� ���׸����� ������
                 * 
                 * State.SetUpdateAction(Action<float> onUpdate) �޼����
                 * this.onUpdate = onUpdate; �Ű����� onUpdate �Լ��� state.onUpdate�� �Ҵ�
                 * Update(Action<T, float> onUpdate) ��� : 
                 * onUpdate �Լ� ���� �Ҵ�
                 */
                .Update((state, deltaTime) =>
                {
                    var directionToTarget = transform.position - goal.position;
                    directionToTarget.y = 0; // Only calculate movement on a 2d plane
                    directionToTarget.Normalize();

                    transform.position -= directionToTarget * deltaTime * state.movementSpeed;
                })
                /* Event(string identifier, Action<T> action) �޼����
                 * state.SetEvent<EventArgs>(identifier, _ => action(state)); �� �����ϰ�
                 * return this; ��
                 * action�� �� �޼ҵ�� �׻�
                 * AbstractState�� ��ӹ��� Ŭ������ ���� �Ű������� �־�߸� �ϵ��� ���׸����� ������
                 * State.SetEvent<TEvent>(string identifier, Action<TEvent> eventTriggeredAction) �޼����
                 * events.Add(identifier, args => eventTriggeredAction(CheckEventArgs<TEvent>(identifier, args)));
                 * events ��ųʸ���, 
                 * Key : identifier 
                 * Value : args => eventTriggeredAction(CheckEventArgs<TEvent>(identifier, args)) 
                 * �߰�
                 * TEvent CheckEventArgs<TEvent>(string identifier, EventArgs args) 
                 * �̰� �׳� args ��ȯ��
                 * Value���� �׻� EventArgs �� ��ӹ��� �Ű�����(args)�� ���� 
                 * eventTriggeredAction �Լ��� ��
                 * 
                 * ���
                 * events ��ųʸ���
                 * Key : identifier 
                 * Value : action
                 * �߰��ϴ� �Լ�
                 * 
                 * �Ʒ� �̺�Ʈ �Լ�(action) ������
                 * state.PushState("Retreat");��
                 * children���� "Retreat" key�� ������ state��
                 * activeChildren�� ����
                 * 
                 * Event(string identifier, Action<T> action) ���: 
                 * events Dictionary��, 
                 * key : identifier , Value : action �߰�
                 */
                .Event("TargetReached", state =>
                {
                    state.PushState("Retreat");
                })
                /* Exit(Action<T> onExit)
                 * state.SetExitAction(() => onExit(state));
                 * return this; �ϴ� �Լ�
                 * state.SetExitAction(() => onExit(state))
                 * this.onExit = onExit;
                 * Exit(Action<T> onExit); ���
                 * state.onExit = onExit; �ϴ� �Լ�
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
                 * parentState.AddChild(state, name); (parentState = parentBuilder(=StateBuilder) �������)
                 * parentState.children.Add(stateName, newState);
                 * newState( = state = new T()).Parent = parentState;
                 * "Retreat" �� ������ State�� parentState�� "Approach"
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
                     * return this; �ϴ� �Լ�
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
                     * conditions ����Ʈ�� Condition ����ü ��ü �߰�
                     * Predicate �� ���ǵ� ����Լ�(��ȯ�� bool)
                     * Action ���� ���ǵ� ����Լ�(��ȯ�� ����)
                     * �̰����� ���ǵ� �Լ�����, state.Update �Լ� ���� ��
                     * �ֱ������� ���� ��
                     * Predicate �Լ��� ���� ����Ǿ�, ������ üũ,
                     * ������ �����ϸ� Action ����
                     * Condition(Func<bool> predicate, Action<T> action) ��� : 
                     * conditions ����Ʈ�� Condition ����ü ��ü �߰�
                     * 
                     */
                    .Condition(() =>
                    {
                        return Vector3.Distance(transform.position, goal.position) >= resetDistance;
                    },
                    state =>
                    {
                        /* activeChildren ������� ��ȯ ��
                         * �� ���.PopState() ���� ��
                         * onExit(); �����ϴ� �Լ���
                         * state�� StateBuilder�� �������
                         * state.Parent = "Retreat" ��
                         * Retreat�� activeChildren.Pop.Exit() ��
                         * ���� state�� activeChildren ����
                         */
                        state.Parent.PopState();

                        //���� state�� Enter ����
                    })
                    .Exit(state =>
                    {
                        Debug.Log("Exiting Retreat state");
                    })
                    .End()//StateBuilder().parentBuilder ��ȯ(State<MovingState>("Approach") ���� ������ StateBuilder)
                .End()//StateBuilder().parentBuilder ��ȯ(���� ������ StateMachineBuilder)
            .Build();//rootState ��ȯ

        /* children ��ųʸ��� "Approach" key �� ����� MovingState ��ȯ �ް�,
         * ���� �������� activeChildren ������
         * activeChildren.Push(newState); �� ��,
         * newState.Enter�Լ� ����
         */
        rootState.ChangeState("Approach");
    }

    // Update is called once per frame
    void Update()
    {
        /* activeChildren�� onUpdate �Լ� ����
         * ��� ���������� activeChildren.Peek().Update(Time.deltaTime); ��
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
            /* TriggerEvent(string name) ����Ǹ�
             * TriggerEvent(name, EventArgs.Empty); ����ǰ�
             * activeChildren.Peek().TriggerEvent(name, eventArgs); �����
             * activeChildren.activeChildren.Count == 0 �� ���̱⿡
             * events ��ųʸ�����, "TargetReached" �� ������ Action �޼ҵ带 ������
             * �ش� �޼ҵ� ����
             * �Ʒ� ���� ����Ǹ�,
             * state.PushState("Retreat"); �� �����
             */
            rootState.TriggerEvent("TargetReached");
        }
    }


}
