# PBAS
Property Based Action System

## 개요
PBAS는 Property와 Action을 사용하여 캐릭터의 행동을 만들고, 실행할 수 있는 패키지입니다.
PBASActor로 이루어진 환경에서 PBASActor가 각자 다양한 Action을 실행하고 환경에 영향을 주며 변화합니다.

Property는 오브젝트의 특성이며 PBASActor는 여러 개의 Property를 가질 수 있습니다.
Action은 PBASActor가 실행할 수 있는 일종의 명령어입니다.
Action은 Action이 실행될 때 전달되는 Parameter, Action을 실행하기 위한 Precondition, Action의 결과로 발생할 Effect로 이루어져 있습니다.
Effect를 사용하여 Parameter로 전달된 PBASActor에 새로운 Property를 추가하거나 기존의 Property를 제거할 수 있고, Property가 가지고 있는 값을 변경할 수 있습니다.

PBASActor가 실행하는 Action에 다양한 Animation을 실행할 필요가 있습니다.
PBASAnimator에 Action에 따라서 원하는 AnimationCommand를 추가하면 Action이 실행될 때 자동으로 AnimationCommand가 실행됩니다.
그리고 자동으로 유니티 Animator의 다음 State가 끝날 때 현재 Action의 Effect가 실행됩니다.


## Aciton 실행 과정
1. PBASActor가 특정 Action을 실행하면 월드에 존재하는 모든 PBASActor의 Property를 수집한뒤 가장 가까운 PBASActor를 Parameter로 전달합니다.
2. Parameter의 Property가 Precondition을 만족하는지 검사합니다.
3. 만족한다면 PBASAnimator를 사용하여 Animation을 재생합니다.
4. Animation이 있는지 여부에 따라서 즉시 혹은 애니메이션이 끝날때 Effect가 실행됩니다.