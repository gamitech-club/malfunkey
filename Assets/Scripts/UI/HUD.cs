using UnityEngine;
using EditorAttributes;

public class HUD : MonoBehaviour
{
    [SerializeField, Required] private Player _player;
    [SerializeField, Required] private KeyImage _keyLeft;
    [SerializeField, Required] private KeyImage _keyDown;
    [SerializeField, Required] private KeyImage _keyUp;
    [SerializeField, Required] private KeyImage _keyRight;

    private void Update()
    {
        HandleKeyImages();
    }

    private void HandleKeyImages()
    {
        var moveInput = _player.MoveInput;
        _keyLeft.SetPressed(moveInput.x < 0);
        _keyDown.SetPressed(_player.IsPoundKeyPressed);
        _keyUp.SetPressed(_player.IsJumpKeyPressed);
        _keyRight.SetPressed(moveInput.x > 0);

        _keyLeft.SetDisabled(_player.LeftKeyAvailable == 0);
        _keyDown.SetDisabled(_player.PoundAvailable == 0);
        _keyUp.SetDisabled(_player.JumpAvailable == 0);
        _keyRight.SetDisabled(_player.RightKeyAvailable == 0);
    }
}
