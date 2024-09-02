/* 04. Simple Coroutine FSM ���� */
using System.Collections;
using UnityEngine;

// �÷��̾ �� �� �ִ� �ൿ (���, �ȱ�, �ٱ�, ����)
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
        // 1~4 ����Ű�� ���� ���� ����
        if (Input.GetKeyDown("1")) ChangeState(PlayerState.Idle);
        else if (Input.GetKeyDown("2")) ChangeState(PlayerState.Walk);
        else if (Input.GetKeyDown("3")) ChangeState(PlayerState.Run);
        else if (Input.GetKeyDown("4")) ChangeState(PlayerState.Attack);
    }

    /// <summary>
    /// �÷��̾��� �ൿ�� newState�� �����Ѵ�.
    /// </summary>
    private void ChangeState(PlayerState newState)
    {
        // ������ ����.ToString()�� �ϰ� �Ǹ� �������� ���ǵ� ���� �̸� string�� ��ȯ
        // playerState = PlayerState.Idle; �� �� playerState.ToString()�� �ϸ� "Idle" ��ȯ

        // �������� ���ǵ� ���¿� ������ �̸��� �ڷ�ƾ �޼ҵ带 ����
        // playerState�� ���� ���¿� ���� �ڷ�ƾ �޼ҵ� ����

        // ���� ������ �ڷ�ƾ ����
        StopCoroutine(playerState.ToString());
        // ���ο� ���·� ����
        playerState = newState;
        // ���� ������ �ڷ�ƾ ����
        StartCoroutine(playerState.ToString());
    }

    private IEnumerator Idle()
    {
        // ���·� ������ �� 1ȸ ȣ���ϴ� ����
        Debug.Log("������ ���� ����");
        Debug.Log("ü��/������ �ʴ� 10�� �ڵ� ȸ��");

        // ���°� ������Ʈ �Ǵ� ���� �� ������ ȣ���ϴ� ����
        while (true)
        {
            Debug.Log("�÷��̾ ���ڸ����� ������Դϴ�.");
            yield return null;
        }
    }

    private IEnumerator Walk()
    {
        // ���·� ������ �� 1ȸ ȣ���ϴ� ����
        Debug.Log("�̵��ӵ��� 2�� �����Ѵ�.");

        // ���°� ������Ʈ �Ǵ� ���� �� ������ ȣ���ϴ� ����
        while (true)
        {
            Debug.Log("�÷��̾ �ɾ�ϴ�.");
            yield return null;
        }
    }

    private IEnumerator Run()
    {
        // ���·� ������ �� 1ȸ ȣ���ϴ� ����
        Debug.Log("�̵��ӵ��� 5�� �����Ѵ�.");

        // ���°� ������Ʈ �Ǵ� ���� �� ������ ȣ���ϴ� ����
        while (true)
        {
            Debug.Log("�÷��̾ �پ�ϴ�.");
            yield return null;
        }
    }

    private IEnumerator Attack()
    {
        // ���·� ������ �� 1ȸ ȣ���ϴ� ����
        Debug.Log("���� ���� ����");
        Debug.Log("�ڵ� ȸ���� �����˴ϴ�.");

        // ���°� ������Ʈ �Ǵ� ���� �� ������ ȣ���ϴ� ����
        while (true)
        {
            Debug.Log("�÷��̾ ��ǥ���� �����մϴ�.");
            yield return null;
        }
    }
}

/* 03. ���·� ������ �� 1ȸ ȣ��Ǵ� ���� �߰�
using UnityEngine;

// �÷��̾ �� �� �ִ� �ൿ (���, �ȱ�, �ٱ�, ����)
public enum PlayerState { Idle = 0, Walk, Run, Attack }

public class PlayerController : MonoBehaviour
{
	private	PlayerState	playerState;

	// ���°� �ٲ� �� true�� �Ǿ� ���� ������Ʈ ���ο��� 1ȸ�� ����Ǵ� ������ �����ϰ� false�� ����
	private	bool		isChanged = false;

	private void Awake()
	{
		ChangeState(PlayerState.Idle);
	}

	private void Update()
	{
		// 1~4 ����Ű�� ���� ���� ����
		if ( Input.GetKeyDown("1") )		ChangeState(PlayerState.Idle);
		else if ( Input.GetKeyDown("2") )	ChangeState(PlayerState.Walk);
		else if ( Input.GetKeyDown("3") )	ChangeState(PlayerState.Run);
		else if ( Input.GetKeyDown("4") )	ChangeState(PlayerState.Attack);

		UpdateState();
	}

	// ���·� ������ �� 1ȸ ȣ���ϴ� ����
	// Idle ���·� �ٲ� �� : ������ ���� ����, ü��/���� �ڵ� ȸ��
	// Walk ���·� �ٲ� �� : �̵��ӵ� ���� (2)
	// Run ���·� �ٲ� �� : �̵��ӵ� ���� (5)
	// Attack ���·� �ٲ� �� : ���� ���� ����, ü��/���� �ڵ� ȸ�� ����

	/// <summary>
	/// playerState�� ���� ���� �÷��̾� �ൿ�� �����Ѵ�.
	/// </summary>
	private void UpdateState()
	{
		if ( playerState == PlayerState.Idle )
		{
			// ���·� ������ �� 1ȸ ȣ���ϴ� ����
			if ( isChanged == true )
			{
				Debug.Log("������ ���� ����");
				Debug.Log("ü��/������ �ʴ� 10�� �ڵ� ȸ��");
				
				isChanged = false;
			}

			// ���°� ������Ʈ �Ǵ� ���� �� ������ ȣ���ϴ� ����
			Debug.Log("�÷��̾ ���ڸ����� ������Դϴ�.");
		}
		else if ( playerState == PlayerState.Walk )
		{
			// ���·� ������ �� 1ȸ ȣ���ϴ� ����
			if ( isChanged == true )
			{
				Debug.Log("�̵��ӵ��� 2�� �����Ѵ�.");
			
				isChanged = false;
			}
			// ���°� ������Ʈ �Ǵ� ���� �� ������ ȣ���ϴ� ����
			Debug.Log("�÷��̾ �ɾ�ϴ�.");
		}
		else if ( playerState == PlayerState.Run )
		{
			// ���·� ������ �� 1ȸ ȣ���ϴ� ����
			if ( isChanged == true )
			{
				Debug.Log("�̵��ӵ��� 5�� �����Ѵ�.");

				isChanged = false;
			}
			// ���°� ������Ʈ �Ǵ� ���� �� ������ ȣ���ϴ� ����
			Debug.Log("�÷��̾ �پ�ϴ�.");
		}
		else if ( playerState == PlayerState.Attack )
		{
			// ���·� ������ �� 1ȸ ȣ���ϴ� ����
			if ( isChanged == true )
			{
				Debug.Log("���� ���� ����");
				Debug.Log("�ڵ� ȸ���� �����˴ϴ�.");

				isChanged = false;
			}
			// ���°� ������Ʈ �Ǵ� ���� �� ������ ȣ���ϴ� ����
			Debug.Log("�÷��̾ ��ǥ���� �����մϴ�.");
		}
	}

	/// <summary>
	/// �÷��̾��� �ൿ�� newState�� �����Ѵ�.
	/// </summary>
	private void ChangeState(PlayerState newState)
	{
		playerState = newState;
		isChanged	= true;
	}
}
 */

/* 02. switch-case�� ������ FSM
 * using UnityEngine;

// �÷��̾ �� �� �ִ� �ൿ (���, �ȱ�, �ٱ�, ����)
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
		// 1~4 ����Ű�� ���� ���� ����
		if ( Input.GetKeyDown("1") )		ChangeState(PlayerState.Idle);
		else if ( Input.GetKeyDown("2") )	ChangeState(PlayerState.Walk);
		else if ( Input.GetKeyDown("3") )	ChangeState(PlayerState.Run);
		else if ( Input.GetKeyDown("4") )	ChangeState(PlayerState.Attack);

		UpdateState();
	}

	/// <summary>
	/// playerState�� ���� ���� �÷��̾� �ൿ�� �����Ѵ�.
	/// </summary>
	private void UpdateState()
	{
		switch ( playerState )
		{
			case PlayerState.Idle:
				Debug.Log("�÷��̾ ���ڸ����� ������Դϴ�.");
				break;
			case PlayerState.Walk:
				Debug.Log("�÷��̾ �ɾ�ϴ�.");
				break;
			case PlayerState.Run:
				Debug.Log("�÷��̾ �پ�ϴ�.");
				break;
			case PlayerState.Attack:
				Debug.Log("�÷��̾ ��ǥ���� �����մϴ�.");
				break;
		}
	}

	/// <summary>
	/// �÷��̾��� �ൿ�� newState�� �����Ѵ�.
	/// </summary>
	private void ChangeState(PlayerState newState)
	{
		playerState = newState;
	}
}
*/

/* 01. if-then���� ������ FSM
 * using UnityEngine;

// �÷��̾ �� �� �ִ� �ൿ (���, �ȱ�, �ٱ�, ����)
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
		// 1~4 ����Ű�� ���� ���� ����
		if ( Input.GetKeyDown("1") )		ChangeState(PlayerState.Idle);
		else if ( Input.GetKeyDown("2") )	ChangeState(PlayerState.Walk);
		else if ( Input.GetKeyDown("3") )	ChangeState(PlayerState.Run);
		else if ( Input.GetKeyDown("4") )	ChangeState(PlayerState.Attack);

		UpdateState();
	}

	/// <summary>
	/// playerState�� ���� ���� �÷��̾� �ൿ�� �����Ѵ�.
	/// </summary>
	private void UpdateState()
	{
		if ( playerState == PlayerState.Idle )
		{
			Debug.Log("�÷��̾ ���ڸ����� ������Դϴ�.");
		}
		else if ( playerState == PlayerState.Walk )
		{
			Debug.Log("�÷��̾ �ɾ�ϴ�.");
		}
		else if ( playerState == PlayerState.Run )
		{
			Debug.Log("�÷��̾ �پ�ϴ�.");
		}
		else if ( playerState == PlayerState.Attack )
		{
			Debug.Log("�÷��̾ ��ǥ���� �����մϴ�.");
		}
	}

	/// <summary>
	/// �÷��̾��� �ൿ�� newState�� �����Ѵ�.
	/// </summary>
	private void ChangeState(PlayerState newState)
	{
		playerState = newState;
	}
}
*/