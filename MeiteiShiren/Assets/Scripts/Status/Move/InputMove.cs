/*=====
<InputMove.cs>

-author
	mizunose

-about
	入力移動を実装
=====*/

// 名前空間宣言
using System.Collections;
using UnityEngine;

// クラス定義
/// <summary>
/// <para>プレイヤー</para>
/// </summary>
[CreateAssetMenu(menuName = "Move/InputMove", fileName = "ExperienceTable")]
public class InputMove : Move
{
	public override void Action()
	{
		StartCoroutine(Motion());
	}


	private IEnumerator Motion()
	{
		Vector3 _from = transform.position;
		Vector3 _at = _from;
		_at.x += InputManager.Instance.Ingame.Player.Move.ReadValue<Vector2>().x * MapData.MASS_SIZE;
		_at.z += InputManager.Instance.Ingame.Player.Move.ReadValue<Vector2>().y * MapData.MASS_SIZE;
		
		//Debug.Log(new Vector2(InputManager.Instance.Ingame.Player.Move.ReadValue<Vector2>().x * MapData.MASS_SIZE, InputManager.Instance.Ingame.Player.Move.ReadValue<Vector2>().x * MapData.MASS_SIZE));

		var aft_mass = Dungeon.Instance.Map.Masses[((int)((_at.z + MapData.MASS_SIZE / 2.0f) / MapData.MASS_SIZE)), ((int)((_at.x + MapData.MASS_SIZE / 2.0f) / MapData.MASS_SIZE))];
		if(!aft_mass || aft_mass.type == Mass.TYPE.WALL)
		{
			yield break;
		}


		float _front = transform.rotation.eulerAngles.y;
		float _direction = Vector2.Angle(Vector2.up, InputManager.Instance.Ingame.Player.Move.ReadValue<Vector2>());
		
		transform.LookAt( _at );
		float _time = 0.0f;
		
		//InputManager.Instance.Ingame.Player.Disable();

		while (true)
		{
			_time += Time.deltaTime;

			

			float _timerate = 0.0f;
			if(_time > MoveStatus.Instance.Spend)
			{
				_timerate = 1.0f;
			}
			else
			{
				_timerate = _time / MoveStatus.Instance.Spend;
			}

			//transform.position = Vector3.Lerp(_from, _at, 1.0f - Mathf.Pow(1.0f - _timerate, 4.0f));
			transform.position = Vector3.Lerp(_from, _at,  - (Mathf.Cos(Mathf.PI * _timerate) - 1.0f ) /2.0f);
			//transform.position = Vector3.Lerp(_from, _at,  _timerate * _timerate);
			//transform.position = Vector3.Lerp(_from, _at, _timerate);

			var _rotation = transform.rotation;
			var _angles = transform.rotation.eulerAngles;
			_angles.y = Mathf.Lerp(_front, _direction, 1.0f - Mathf.Pow(1.0f - _timerate, 4.0f));
			_rotation.eulerAngles = _angles;
			transform.rotation = _rotation;

			if (_timerate == 1.0f)
			{
				//InputManager.Instance.Ingame.Player.Enable();
				break;
			}
			else
			{
				yield return null;
			}
		}

		
	}
}