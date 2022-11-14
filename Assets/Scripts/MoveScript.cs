using UnityEngine;

/// <summary>
/// Simply moves the current game object
/// </summary>
public class MoveScript : MonoBehaviour
{
  // 0 - Designer variables

  /// <summary>
  /// Projectile speed
  /// </summary>
  public Vector2 speed = new Vector2(10, 10);

  /// <summary>
  /// Moving direction
  /// </summary>
  public Vector2 direction = new Vector2(-1, 0);
	
  private Vector2 movement;

  void Update()
  {
    // 1 - Movement
    movement = new Vector3(
      0,
      speed.y * direction.y, speed.x * direction.x);
  }

  void FixedUpdate()
  {
	// Apply movement to the rigidbody
	GetComponent<Rigidbody2D>().velocity = movement;
  }
}