/*=====
<VirtualizeMono.cs>

-author
	mizunose

-about
	MonoBehaviorのイベント関数を、子クラスがsealed修飾子をつけれるように仮想化

-note
・MonoBehaviorの各イベント関数をsealedすることができるようになります
	書き換えは防ぎたいがそのタイミングで追加で処理したいものがあればそれぞれここで仮想的に定義されている関数(Custom~~()関数)を使うのが早いと思います。
	なお、ここで定義されているイベントでない関数は定義のみであり、あくまで各子クラスで定義しなおす手間を省いているにすぎません。
	呼び出しはされていないのでsealedを定義する際に呼び出してください。
・protectedの修飾子は変更が効きません	：	これをいじらなければならない場合はこのクラスの使用はお控えください。
	(publicがないのはイベント関数をイベントらしく実装するため、privateがないのはオーバーロードを防ぐためです)
=====*/

// 名前空間宣言
using UnityEngine;

// クラス定義

/// <summary>
/// <para>イベント関数仮想化クラス</para>
/// </summary>
public class VirtualizeMono : MonoBehaviour
{
	/// <summary>
	/// <para>インスタンス生成直後に行う処理</para>
	/// </summary>
	virtual protected void Awake() { }

	/// <summary>
	/// <para>Awake()関数で任意に処理を追加しやすいように定義</para>
	/// </summary>
	virtual protected void CustomAwake() { }

	/// <summary>
	/// <para>初回更新直前に行う処理</para>
	/// </summary>
	virtual protected void Start() { }

	/// <summary>
	/// <para>Start()関数で任意に処理を追加しやすいように定義</para>
	/// </summary>
	virtual protected void CustomStart() { }

	/// <summary>
	/// <para>自身のオブジェクトが有効になった瞬間に行う処理</para>
	/// </summary>
	virtual protected void OnEnable() { }

	/// <summary>
	/// <para>OnEnable()関数で任意に処理を追加しやすいように定義</para>
	/// </summary>
	virtual protected void CustomOnEnable() { }

	/// <summary>
	/// <para>一定時間ごとに行う更新処理</para>
	/// </summary>
	virtual protected void Update() { }

	/// <summary>
	/// <para>Update()関数で任意に処理を追加しやすいように定義</para>
	/// </summary>
	virtual protected void CustomUpdate() { }

	/// <summary>
	/// <para>各フレームにおいて通常のUpdate関数の後に行う更新処理</para>
	/// </summary>
	virtual protected void LateUpdate() { }

	/// <summary>
	/// <para>LateUpdate()関数で任意に処理を追加しやすいように定義</para>
	/// </summary>
	virtual protected void CustomLateUpdate() { }

	/// <summary>
	/// <para>一定時間ごとに行う物理的な更新処理</para>
	/// </summary>
	virtual protected void FixedUpdate() { }

	/// <summary>
	/// <para>FixedUpdate()関数で任意に処理を追加しやすいように定義</para>
	/// </summary>
	virtual protected void CustomFixedUpdate() { }

	/// <summary>
	/// <para>インスタンス破棄時に行う処理</para>
	/// </summary>
	virtual protected void OnDestroy() { }

	/// <summary>
	/// <para>OnDestroy()関数で任意に処理を追加しやすいように定義</para>
	/// </summary>
	virtual protected void CustomOnDestroy() { }

	/// <summary>
	/// <para>自身のオブジェクトが無効になった瞬間に行う処理</para>
	/// </summary>
	virtual protected void OnDisable() { }

	/// <summary>
	/// <para>OnDisable()関数で任意に処理を追加しやすいように定義</para>
	/// </summary>
	virtual protected void CustomOnDisable() { }

	/// <summary>
	/// <para>任意のカメラに映り始めた瞬間に行う処理</para>
	/// </summary>
	virtual protected void OnBecameVisible() { }

	/// <summary>
	/// <para>OnBecameVisible()関数で任意に処理を追加しやすいように定義</para>
	/// </summary>
	virtual protected void CustomOnBecameVisible() { }

	/// <summary>
	/// <para>任意のカメラに映らなくなった瞬間に行う処理</para>
	/// </summary>
	virtual protected void OnBecameInvisible() { }

	/// <summary>
	/// <para>OnBecameInvisible()関数で任意に処理を追加しやすいように定義</para>
	/// </summary>
	virtual protected void CustomOnBecameInvisible() { }

	/// <summary>
	/// <para>映っている間カメラごとに呼び出される処理(IsTrigger off時)</para>
	/// </summary>
	virtual protected void OnWillRenderObject() { }

	/// <summary>
	/// <para>OnWillRenderObject()関数で任意に処理を追加しやすいように定義</para>
	/// </summary>
	virtual protected void CustomOnWillRenderObject() { }

	/// <summary>
	/// <para>3D空間上で接触の当たり判定が取られた瞬間に行う処理(IsTrigger off時)</para>
	/// </summary>
	/// <param name="_Collision">接触相手</param>
	virtual protected void OnCollisionEnter(Collision _Collision) { }

	/// <summary>
	/// <para>OnCollisionEnter()関数で任意に処理を追加しやすいように定義</para>
	/// </summary>
	/// <param name="_Collision">接触相手</param>
	virtual protected void CustomOnCollisionEnter(Collision _Collision) { }

	/// <summary>
	/// <para>2D空間上で接触の当たり判定が取られた瞬間に行う処理(IsTrigger off時)</para>
	/// </summary>
	/// <param name="_Collision">接触相手</param>
	virtual protected void OnCollisionEnter2D(Collision2D _Collision) { }

	/// <summary>
	/// <para>OnCollisionEnter2D()関数で任意に処理を追加しやすいように定義</para>
	/// </summary>
	/// <param name="_Collision">接触相手</param>
	virtual protected void CustomOnCollisionEnter2D(Collision2D _Collision) { }

	/// <summary>
	/// <para>3D空間上で接触の当たり判定が取られた瞬間に行う処理(IsTrigger on時)</para>
	/// </summary>
	/// <param name="_Collider">接触相手</param>
	virtual protected void OnTriggerEnter(Collider _Collider) { }

	/// <summary>
	/// <para>OnTriggerEnter()関数で任意に処理を追加しやすいように定義</para>
	/// </summary>
	/// <param name="_Collider">接触相手</param>
	virtual protected void CustomOnTriggerEnter(Collider _Collider) { }

	/// <summary>
	/// <para>2D空間上で接触の当たり判定が取られた瞬間に行う処理(IsTrigger on時)</para>
	/// </summary>
	/// <param name="_Collider">接触相手</param>
	virtual protected void OnTriggerEnter2D(Collider2D _Collider) { }

	/// <summary>
	/// <para>OnTriggerEnter2D()関数で任意に処理を追加しやすいように定義</para>
	/// </summary>
	/// <param name="_Collider">接触相手</param>
	virtual protected void CustomOnTriggerEnter2D(Collider2D _Collider) { }

	/// <summary>
	/// <para>3D空間上で接触の当たり判定が取られている間行う処理(IsTrigger off時)</para>
	/// </summary>
	/// <param name="_Collision">接触相手</param>
	virtual protected void OnCollisionStay(Collision _Collision) { }

	/// <summary>
	/// <para>OnCollisionStay()関数で任意に処理を追加しやすいように定義</para>
	/// </summary>
	/// <param name="_Collision">接触相手</param>
	virtual protected void CustomOnCollisionStay(Collision _Collision) { }

	/// <summary>
	/// <para>2D空間上で接触の当たり判定が取られている間行う処理(IsTrigger off時)</para>
	/// </summary>
	/// <param name="_Collision">接触相手</param>
	virtual protected void OnCollisionStay2D(Collision2D _Collision) { }

	/// <summary>
	/// <para>OnCollisionStay2D()関数で任意に処理を追加しやすいように定義</para>
	/// </summary>
	/// <param name="_Collision">接触相手</param>
	virtual protected void CustomOnCollisionStay2D(Collision2D _Collision) { }

	/// <summary>
	/// <para>3D空間上で接触の当たり判定が取られている間行う処理(IsTrigger on時)</para>
	/// </summary>
	/// <param name="_Collider">接触相手</param>
	virtual protected void OnTriggerStay(Collider _Collider) { }

	/// <summary>
	/// <para>OnTriggerStay()関数で任意に処理を追加しやすいように定義</para>
	/// </summary>
	/// <param name="_Collider">接触相手</param>
	virtual protected void CustomOnTriggerStay(Collider _Collider) { }

	/// <summary>
	/// <para>2D空間上で接触の当たり判定が取られている間行う処理(IsTrigger on時)</para>
	/// </summary>
	/// <param name="_Collider">接触相手</param>
	virtual protected void OnTriggerStay2D(Collider2D _Collider) { }

	/// <summary>
	/// <para>OnTriggerStay2D()関数で任意に処理を追加しやすいように定義</para>
	/// </summary>
	/// <param name="_Collider">接触相手</param>
	virtual protected void CustomOnTriggerStay2D(Collider2D _Collider) { }

	/// <summary>
	/// <para>3D空間上で接触していた物体と離れた瞬間に行う処理(IsTrigger off時)</para>
	/// </summary>
	/// <param name="_Collision">接触相手</param>
	virtual protected void OnCollisionExit(Collision _Collision) { }

	/// <summary>
	/// <para>OnCollisionExit()関数で任意に処理を追加しやすいように定義</para>
	/// </summary>
	/// <param name="_Collision">接触相手</param>
	virtual protected void CustomOnCollisionExit(Collision _Collision) { }

	/// <summary>
	/// <para>2D空間上で接触していた物体と離れた瞬間に行う処理(IsTrigger off時)</para>
	/// </summary>
	/// <param name="_Collision">接触相手</param>
	virtual protected void OnCollisionExit2D(Collision2D _Collision) { }

	/// <summary>
	/// <para>OnCollisionExit2D()関数で任意に処理を追加しやすいように定義</para>
	/// </summary>
	/// <param name="_Collision">接触相手</param>
	virtual protected void CustomOnCollisionExit2D(Collision2D _Collision) { }

	/// <summary>
	/// <para>3D空間上で接触していた物体と離れた瞬間に行う処理(IsTrigger on時)</para>
	/// </summary>
	/// <param name="_Collider">接触相手</param>
	virtual protected void OnTriggerExit(Collider _Collider) { }

	/// <summary>
	/// <para>OnTriggerExit()関数で任意に処理を追加しやすいように定義</para>
	/// </summary>
	/// <param name="_Collider">接触相手</param>
	virtual protected void CustomOnTriggerExit(Collider _Collider) { }

	/// <summary>
	/// <para>2D空間上で接触していた物体と離れた瞬間に行う処理(IsTrigger on時)</para>
	/// </summary>
	/// <param name="_Collider">接触相手</param>
	virtual protected void OnTriggerExit2D(Collider2D _Collider) { }

	/// <summary>
	/// <para>OnTriggerExit2D()関数で任意に処理を追加しやすいように定義</para>
	/// </summary>
	/// <param name="_Collider">接触相手</param>
	virtual protected void CustomOnTriggerExit2D(Collider2D _Collider) { }

	/// <summary>
	/// <para>パーティクルがコライダーと接触している間行う処理(SendCollisionMessage on時)</para>
	/// </summary>
	/// <param name="_GameObject">当たったオブジェクトの情報</param>
	virtual protected void OnParticleCollision(GameObject _GameObject) { }

	/// <summary>
	/// <para>OnParticleCollision()関数で任意に処理を追加しやすいように定義</para>
	/// </summary>
	/// <param name="_GameObject">当たったオブジェクトの情報</param>
	virtual protected void CustomOnParticleCollision(GameObject _GameObject) { }

	/// <summary>
	/// <para>ParticleSystemがTriggersモジュールを搭載している間呼び出される処理</para>
	/// </summary>
	virtual protected void OnParticleTrigger() { }

	/// <summary>
	/// <para>OnParticleTrigger()関数で任意に処理を追加しやすいように定義</para>
	/// </summary>
	virtual protected void CustomOnParticleTrigger() { }
}