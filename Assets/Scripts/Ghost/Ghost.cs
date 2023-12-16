using System;
using System.Collections.Generic;
using UnityEngine;


public class Ghost : MonoBehaviour
{

    [SerializeField] float sightRadius = 3f; //시야범위(반지름)
    [SerializeField] float sightAngle = 45f;//시야 각도
    [SerializeField] LayerMask targetLayer; //도중에 감지되면 따라갈 대상
    [SerializeField] LayerMask collisionLayer; //충돌레이어
    [SerializeField] float moveRadius = 10f; //이동 반경
    [SerializeField] float moveSpeed = 1f;//이동 속도
    float catchingDistance = 0.1f; //타겟과의 접근거리
    Vector3 targetPosition;
    [SerializeField] float invisibleTime = 0.5f;//투명화 지속시간
    [SerializeField] float visibleTime = 0.5f;//실체화 지속시간
    [SerializeField] bool isVisible = true;
    [SerializeField] float visibleStateCount = 0f;

    [SerializeField] float maxHuntingGraceTime = 25f;//헌팅 최대 유예시간
    [SerializeField] float currentHuntingGraceTime = 25f;//헌팅 유예시간
    [SerializeField] float huntingMentalityStat = 50f;//헌팅시작 정신력
    [SerializeField] float maxHuntingDuration = 15f;//헌팅 지속시간
    [SerializeField] float currentHuntingDuration = 15f;//헌팅 지속시간
    [SerializeField] int evidenceAmount = 3;//증거 개수
    
    //=========================이렇게 넣는거 나중에 바꿀 수 있으면 바꾸기
    const string dotprojector = "dotprojector";
    const string EMFLevel5 = "EMFLevel5";
    const string FreezingTemperatures = "FreezingTemperatures";
    const string GhostOrb = "GhostOrb";
    const string GhostWriting = "GhostWriting";
    const string SpiritBox = "SpiritBox";
    const string Ultraviolet = "Ultraviolet";
    string[] evidenceString =
    {
        dotprojector,
        EMFLevel5,
        FreezingTemperatures,
        GhostOrb,
        GhostWriting,
        SpiritBox,
        Ultraviolet
    };
    //=========================이렇게 넣는거 나중에 바꿀 수 있으면 바꾸기
    //=================================================
    public List<string> Banshee = new List<string>
    {
        dotprojector,
        GhostOrb,
        Ultraviolet
    };
    public List<string> Demon = new List<string>
    {
        GhostWriting,
        Ultraviolet,
        FreezingTemperatures
    };
    public List<string> Deogen = new List<string>
    {
        dotprojector,
        GhostWriting,
        SpiritBox
    };
    public List<string> Goryo = new List<string>
    {
        dotprojector, EMFLevel5, Ultraviolet
    };
    public List<string> Hantu = new List<string>
    {
        GhostOrb, Ultraviolet, FreezingTemperatures
    };
    public List<string> Jinn = new List<string>
    {
        EMFLevel5, Ultraviolet, FreezingTemperatures
    };
    public List<string> Mare = new List<string>
    {
        GhostWriting, GhostOrb, SpiritBox
    };
    public List<string> Moroi = new List<string>
    {
        GhostWriting, FreezingTemperatures, SpiritBox
    };
    public List<string> Myling = new List<string>
    {
        GhostWriting, EMFLevel5, Ultraviolet
    };
    public List<string> Obake = new List<string>
    {
        EMFLevel5, GhostOrb, Ultraviolet
    };
    public List<string> Oni = new List<string>
    {
        dotprojector, EMFLevel5, FreezingTemperatures
    };
    public List<string> Onryo = new List<string>
    {
        GhostOrb, FreezingTemperatures, SpiritBox
    };
    public List<string> Phantom = new List<string>
    {
        dotprojector, Ultraviolet, SpiritBox
    };
    public List<string> Poltergeist = new List<string>
    {
        GhostWriting, Ultraviolet, SpiritBox
    };
    public List<string> Raiju = new List<string>
    {
        dotprojector, EMFLevel5, GhostOrb
    };
    public List<string> Revenant = new List<string>
    {
        GhostWriting, GhostOrb, FreezingTemperatures
    };
    //public List<string> Shade = new List<string>
    //{

    //};
    //public List<string> Spirit = new List<string>
    //{

    //};
    //public List<string> Thaye = new List<string>
    //{

    //};
    //
    //=================================================

    public enum GhostEvidences
    {
        dotprojector = 0,
        EMFLevel5 = 1,
        FreezingTemperatures = 2,
        GhostOrb = 3,
        GhostWriting = 4,
        SpiritBox = 5, //응답
        Ultraviolet = 6
    }

    public struct EventCondition
    {
        public Func<bool> Predicate;
        public Action Action;
    }

    public Dictionary<GhostEvidences, EventCondition> ghostEvents = new Dictionary<GhostEvidences, EventCondition>();//증거저장용

    public float SightRadius
    {
        get { return sightRadius; }
    }
    public float SightAngle
    {  
        get { return sightAngle; } 
    }
    public LayerMask TargetLayer
    {
        get { return targetLayer; }
    }
    public LayerMask CollisionLayer
    {
        get { return collisionLayer; }
    }
    public float MoveRadius
    {
        get { return moveRadius; }
    }
    public float MoveSpeed
    {
        get { return moveSpeed; }  
    }
    public float CatchingDistance
    {
        get { return catchingDistance; }
        set { catchingDistance = value; }
    }
    public Vector3 TargetPosition
    {
        get { return targetPosition; }
        set { targetPosition = value; }
    }
    public float VisibleTime
    {
        get { return visibleTime; }
        set { visibleTime = value; }
    }
    public float InvisibleTime
    {
        get { return invisibleTime; }
        set { invisibleTime = value; }
    }
    public bool IsVisible
    {
        get { return isVisible; }
        set { isVisible = value; }
    }
    public float VisibleStateCount
    {
        get { return visibleStateCount; }
        set { visibleStateCount = value; }
    }
    public float MaxHuntingGraceTime
    {
        get { return maxHuntingGraceTime; }
        set { maxHuntingGraceTime = value; }
    }
    public float CurrentHuntingGraceTime
    {
        get { return currentHuntingGraceTime; }
        set { currentHuntingGraceTime = value; }
    }
    public float HuntingMentalityStat
    {
        get { return huntingMentalityStat; }
        set { huntingMentalityStat = value; }
    }
    public float MaxHuntingDuration
    {
        get { return maxHuntingDuration; }
        set { maxHuntingDuration = value; }
    }
    public float CurrentHuntingDuration
    {
        get { return currentHuntingDuration; }
        set { currentHuntingDuration = value; }
    }
    public int EvidenceAmount
    {
        get { return evidenceAmount; }
        set { evidenceAmount = value; }
    }

    public string[] EvidenceString
    {
        get { return evidenceString; }
        set { evidenceString = value; }
    }


    //귀신 증거 설정
    public void setGhostEvents(int evidenceAmount)
    {
        GhostEvidences[] EvidenceArray = GetRandomEnumValues<GhostEvidences>(evidenceAmount);
        GhostEventController eventController = transform.GetComponent<GhostEventController>();
        foreach (GhostEvidences evidences in EvidenceArray)
        {
            switch (evidences)
            {
                case GhostEvidences.dotprojector:
                    ghostEvents.Add(evidences, new EventCondition()
                    {
                        Predicate = () => true,
                        Action = () => eventController.dotProjector()
                    });
                    break;
                case GhostEvidences.EMFLevel5:
                    ghostEvents.Add(evidences, new EventCondition()
                    {
                        Predicate = () => true,
                        Action = () => eventController.EMFLevel5()
                    });
                    break;
                case GhostEvidences.FreezingTemperatures:
                    ghostEvents.Add(evidences, new EventCondition()
                    {
                        Predicate = () => true,
                        Action = () => eventController.FreezingTemperatures()
                    });
                    break;
                case GhostEvidences.GhostOrb:
                    ghostEvents.Add(evidences, new EventCondition()
                    {
                        Predicate = () => true,
                        Action = () => eventController.GhostOrb()
                    });
                    break;
                case GhostEvidences.GhostWriting:
                    ghostEvents.Add(evidences, new EventCondition()
                    {
                        Predicate = () => true,
                        Action = () => eventController.GhostWriting()
                    });
                    break;
                case GhostEvidences.SpiritBox:
                    ghostEvents.Add(evidences, new EventCondition()
                    {
                        Predicate = () => true,
                        Action = () => eventController.SpiritBox()
                    });
                    break;
                case GhostEvidences.Ultraviolet:
                    ghostEvents.Add(evidences, new EventCondition()
                    {
                        Predicate = () => true,
                        Action = () => eventController.Ultraviolet()
                    });
                    break;
            }
            Debug.Log(evidences);
        }
    }
    public T[] GetRandomEnumValues<T>(int count)
    {
        System.Array enumValues = System.Enum.GetValues(typeof(T));
        System.Random random = new System.Random();

        // 중복을 피하기 위해 HashSet 사용
        HashSet<T> selectedValues = new HashSet<T>();

        while (selectedValues.Count < count)
        {
            T randomEnumValue = (T)enumValues.GetValue(random.Next(enumValues.Length));
            selectedValues.Add(randomEnumValue);
        }

        // HashSet을 배열로 변환하여 반환
        T[] resultArray = new T[count];
        selectedValues.CopyTo(resultArray);

        return resultArray;
    }

}
