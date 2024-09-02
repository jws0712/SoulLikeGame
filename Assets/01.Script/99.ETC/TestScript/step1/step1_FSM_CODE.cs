/* 04. Simple Coroutine FSM 구현 */
using System.Collections;
using UnityEngine;

// 플레이어가 할 수 있는 행동 (대기, 걷기, 뛰기, 공격)
public enum PlayerState { Idle = 0, Walk, Run, Attack }

public class step1_FSM_CODE : MonoBehaviour
{
    private PlayerState playerState;

    private void Awake()
    {
        ChangeState(PlayerState.Idle);
    }

    private void Update()
    {
        // 1~4 숫자키를 눌러 상태 변경
        if (Input.GetKeyDown("1")) ChangeState(PlayerState.Idle);
        else if (Input.GetKeyDown("2")) ChangeState(PlayerState.Walk);
        else if (Input.GetKeyDown("3")) ChangeState(PlayerState.Run);
        else if (Input.GetKeyDown("4")) ChangeState(PlayerState.Attack);
    }

    /// <summary>
    /// 플레이어의 행동을 newState로 변경한다.
    /// </summary>
    private void ChangeState(PlayerState newState)
    {
        // 열거형 변수.ToString()을 하게 되면 열거형에 정의된 변수 이름 string을 반환
        // playerState = PlayerState.Idle; 일 때 playerState.ToString()을 하면 "Idle" 반환

        // 열거형에 정의된 상태와 동일한 이름의 코루틴 메소드를 정의
        // playerState의 현재 상태에 따라 코루틴 메소드 실행

        // 이전 상태의 코루틴 종료
        StopCoroutine(playerState.ToString());
        // 새로운 상태로 변경
        playerState = newState;
        // 현재 상태의 코루틴 실행
        StartCoroutine(playerState.ToString());
    }

    private IEnumerator Idle()
    {
        // 상태로 진입할 때 1회 호출하는 내용
        Debug.Log("비전투 모드로 변경");
        Debug.Log("체력/마력이 초당 10씩 자동 회복");

        // 상태가 업데이트 되는 동안 매 프레임 호출하는 내용
        while (true)
        {
            Debug.Log("플레이어가 제자리에서 대기중입니다.");
            yield return null;
        }
    }

    private IEnumerator Walk()
    {
        // 상태로 진입할 때 1회 호출하는 내용
        Debug.Log("이동속도를 2로 설정한다.");

        // 상태가 업데이트 되는 동안 매 프레임 호출하는 내용
        while (true)
        {
            Debug.Log("플레이어가 걸어갑니다.");
            yield return null;
        }
    }

    private IEnumerator Run()
    {
        // 상태로 진입할 때 1회 호출하는 내용
        Debug.Log("이동속도를 5로 설정한다.");

        // 상태가 업데이트 되는 동안 매 프레임 호출하는 내용
        while (true)
        {
            Debug.Log("플레이어가 뛰어갑니다.");
            yield return null;
        }
    }

    private IEnumerator Attack()
    {
        // 상태로 진입할 때 1회 호출하는 내용
        Debug.Log("전투 모드로 변경");
        Debug.Log("자동 회복이 중지됩니다.");

        // 상태가 업데이트 되는 동안 매 프레임 호출하는 내용
        while (true)
        {
            Debug.Log("플레이어가 목표물을 공격합니다.");
            yield return null;
        }
    }
}

/* 03. 상태로 진입할 때 1회 호출되는 내용 추가
using UnityEngine;

// 플레이어가 할 수 있는 행동 (대기, 걷기, 뛰기, 공격)
public enum PlayerState { Idle = 0, Walk, Run, Attack }

public class PlayerController : MonoBehaviour
{
	private	PlayerState	playerState;

	// 상태가 바뀔 때 true가 되어 상태 업데이트 내부에서 1회만 실행되는 내용을 실행하고 false로 변경
	private	bool		isChanged = false;

	private void Awake()
	{
		ChangeState(PlayerState.Idle);
	}

	private void Update()
	{
		// 1~4 숫자키를 눌러 상태 변경
		if ( Input.GetKeyDown("1") )		ChangeState(PlayerState.Idle);
		else if ( Input.GetKeyDown("2") )	ChangeState(PlayerState.Walk);
		else if ( Input.GetKeyDown("3") )	ChangeState(PlayerState.Run);
		else if ( Input.GetKeyDown("4") )	ChangeState(PlayerState.Attack);

		UpdateState();
	}

	// 상태로 진입할 때 1회 호출하는 내용
	// Idle 상태로 바뀔 때 : 비전투 모드로 변경, 체력/마력 자동 회복
	// Walk 상태로 바뀔 때 : 이동속도 설정 (2)
	// Run 상태로 바뀔 때 : 이동속도 설정 (5)
	// Attack 상태로 바뀔 때 : 전투 모드로 변경, 체력/마력 자동 회복 중지

	/// <summary>
	/// playerState에 따라 현재 플레이어 행동을 실행한다.
	/// </summary>
	private void UpdateState()
	{
		if ( playerState == PlayerState.Idle )
		{
			// 상태로 진입할 때 1회 호출하는 내용
			if ( isChanged == true )
			{
				Debug.Log("비전투 모드로 변경");
				Debug.Log("체력/마력이 초당 10씩 자동 회복");
				
				isChanged = false;
			}

			// 상태가 업데이트 되는 동안 매 프레임 호출하는 내용
			Debug.Log("플레이어가 제자리에서 대기중입니다.");
		}
		else if ( playerState == PlayerState.Walk )
		{
			// 상태로 진입할 때 1회 호출하는 내용
			if ( isChanged == true )
			{
				Debug.Log("이동속도를 2로 설정한다.");
			
				isChanged = false;
			}
			// 상태가 업데이트 되는 동안 매 프레임 호출하는 내용
			Debug.Log("플레이어가 걸어갑니다.");
		}
		else if ( playerState == PlayerState.Run )
		{
			// 상태로 진입할 때 1회 호출하는 내용
			if ( isChanged == true )
			{
				Debug.Log("이동속도를 5로 설정한다.");

				isChanged = false;
			}
			// 상태가 업데이트 되는 동안 매 프레임 호출하는 내용
			Debug.Log("플레이어가 뛰어갑니다.");
		}
		else if ( playerState == PlayerState.Attack )
		{
			// 상태로 진입할 때 1회 호출하는 내용
			if ( isChanged == true )
			{
				Debug.Log("전투 모드로 변경");
				Debug.Log("자동 회복이 중지됩니다.");

				isChanged = false;
			}
			// 상태가 업데이트 되는 동안 매 프레임 호출하는 내용
			Debug.Log("플레이어가 목표물을 공격합니다.");
		}
	}

	/// <summary>
	/// 플레이어의 행동을 newState로 변경한다.
	/// </summary>
	private void ChangeState(PlayerState newState)
	{
		playerState = newState;
		isChanged	= true;
	}
}
 */

/* 02. switch-case로 구현된 FSM
 * using UnityEngine;

// 플레이어가 할 수 있는 행동 (대기, 걷기, 뛰기, 공격)
public enum PlayerState { Idle = 0, Walk, Run, Attack }

public class PlayerController : MonoBehaviour
{
	private	PlayerState	playerState;

	private void Awake()
	{
		ChangeState(PlayerState.Idle);
	}

	private void Update()
	{
		// 1~4 숫자키를 눌러 상태 변경
		if ( Input.GetKeyDown("1") )		ChangeState(PlayerState.Idle);
		else if ( Input.GetKeyDown("2") )	ChangeState(PlayerState.Walk);
		else if ( Input.GetKeyDown("3") )	ChangeState(PlayerState.Run);
		else if ( Input.GetKeyDown("4") )	ChangeState(PlayerState.Attack);

		UpdateState();
	}

	/// <summary>
	/// playerState에 따라 현재 플레이어 행동을 실행한다.
	/// </summary>
	private void UpdateState()
	{
		switch ( playerState )
		{
			case PlayerState.Idle:
				Debug.Log("플레이어가 제자리에서 대기중입니다.");
				break;
			case PlayerState.Walk:
				Debug.Log("플레이어가 걸어갑니다.");
				break;
			case PlayerState.Run:
				Debug.Log("플레이어가 뛰어갑니다.");
				break;
			case PlayerState.Attack:
				Debug.Log("플레이어가 목표물을 공격합니다.");
				break;
		}
	}

	/// <summary>
	/// 플레이어의 행동을 newState로 변경한다.
	/// </summary>
	private void ChangeState(PlayerState newState)
	{
		playerState = newState;
	}
}
*/

/* 01. if-then으로 구현된 FSM
 * using UnityEngine;

// 플레이어가 할 수 있는 행동 (대기, 걷기, 뛰기, 공격)
public enum PlayerState { Idle = 0, Walk, Run, Attack }

public class PlayerController : MonoBehaviour
{
	private	PlayerState	playerState;

	private void Awake()
	{
		ChangeState(PlayerState.Idle);
	}

	private void Update()
	{
		// 1~4 숫자키를 눌러 상태 변경
		if ( Input.GetKeyDown("1") )		ChangeState(PlayerState.Idle);
		else if ( Input.GetKeyDown("2") )	ChangeState(PlayerState.Walk);
		else if ( Input.GetKeyDown("3") )	ChangeState(PlayerState.Run);
		else if ( Input.GetKeyDown("4") )	ChangeState(PlayerState.Attack);

		UpdateState();
	}

	/// <summary>
	/// playerState에 따라 현재 플레이어 행동을 실행한다.
	/// </summary>
	private void UpdateState()
	{
		if ( playerState == PlayerState.Idle )
		{
			Debug.Log("플레이어가 제자리에서 대기중입니다.");
		}
		else if ( playerState == PlayerState.Walk )
		{
			Debug.Log("플레이어가 걸어갑니다.");
		}
		else if ( playerState == PlayerState.Run )
		{
			Debug.Log("플레이어가 뛰어갑니다.");
		}
		else if ( playerState == PlayerState.Attack )
		{
			Debug.Log("플레이어가 목표물을 공격합니다.");
		}
	}

	/// <summary>
	/// 플레이어의 행동을 newState로 변경한다.
	/// </summary>
	private void ChangeState(PlayerState newState)
	{
		playerState = newState;
	}
}
*/