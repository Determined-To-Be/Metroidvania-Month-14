using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MVMXIV;

namespace MVMXIV
{
    public enum Buttons
    {
        RIGHT   = 0x00000001,
        LEFT    = 0x00000002,
        UP      = 0x00000004,
        DOWN    = 0x00000008,
        FIRE1   = 0x00000010,
        FIRE2   = 0x00000020,
        FIRE3   = 0x00000040,
        JUMP    = 0x00000080,
        SUBMIT  = 0x00000100,
        CANCEL  = 0x00000200
    }

    public enum Axes
    {
        HORIZONTAL,
        VERTICAL
    }
}

public class InputManager : SingletonPattern<InputManager>
{
    InputFrame[] buffer = new InputFrame[30]; // Half-second buffer is reasonable (assuming 60fps)

    void Start()
    {
        // Initialize input buffer
        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = new InputFrame(true);
        }
    }

    void Update()
    {
        // Shift the buffer
        for (int i = buffer.Length - 1; i > 0; i--)
        {
            buffer[i] = buffer[i - 1];
        }
        // Get the latest input data
        buffer[0] = new InputFrame();
    }

    /// <summary>
    /// The method for retrieving an analog input
    /// e.g. Anything with a potentiometer like a stick or a trigger
    /// </summary>
    /// <param name="axis">A valid axis number</param>
    /// <returns>A value between -1.0f and 1.0f representing the current state of the axis</returns>
    public float GetAxis(int axis)
    {
        return buffer[0].GetAxis(axis);
    }

    /// <summary>
    /// The method for retrieving an analog input
    /// e.g. Anything with a potentiometer like a stick or a trigger
    /// </summary>
    /// <param name="axis">A valid axis</param>
    /// <returns>A value between -1.0f and 1.0f representing the current state of the axis</returns>
    public float GetAxis(Axes axis)
    {
        return GetAxis((int)axis);
    }

    /// <summary>
    /// The method for measuring how much an analog input has changed in the given number of input frames
    /// </summary>
    /// <param name="axis">A valid axis number</param>
    /// <param name="deltaFrames">The number of frames within the input buffer to measure the change across</param>
    /// <returns>A value in the range of -2.0f to 2.0f representing a displacement with negative values being a displacement in the opposite direction</returns>
    public float GetAxisDelta(int axis, int deltaFrames)
    {
        deltaFrames = AssertDeltaFrames(deltaFrames);
        return buffer[deltaFrames].GetAxis(axis) - buffer[0].GetAxis(axis);
    }

    /// <summary>
    /// The method for measuring how much an analog input has changed in the given number of input frames
    /// </summary>
    /// <param name="axis">A valid axis</param>
    /// <param name="deltaFrames">The number of frames within the input buffer to measure the change across</param>
    /// <returns>A value in the range of -2.0f to 2.0f representing a displacement with negative values being a displacement in the opposite direction</returns>
    public float GetAxisDelta(Axes axis, int deltaFrames)
    {
        return GetAxisDelta((int)axis, deltaFrames);
    }

    /// <summary>
    /// The method for measuring how much an analog input has changed since the previous input frame
    /// </summary>
    /// <param name="axis">A valid axis number</param>
    /// <returns>A value in the range of -2.0f to 2.0f representing a displacement with negative values being a displacement in the opposite direction</returns>
    public float GetAxisDelta(int axis)
    {
        return GetAxisDelta(axis, 1);
    }

    /// <summary>
    /// The method for measuring how much an analog input has changed since the previous input frame
    /// </summary>
    /// <param name="axis">A valid axis</param>
    /// <returns>A value in the range of -2.0f to 2.0f representing a displacement with negative values being a displacement in the opposite direction</returns>
    public float GetAxisDelta(Axes axis)
    {
        return GetAxisDelta((int)axis, 1);
    }

    /// <summary>
    /// The method for detecting if one or more buttons are currently held
    /// </summary>
    /// <param name="mask">
    /// The button(s) as a mask. The mask can be created using elements of the button enum
    /// (<c>MVMXIV.Buttons</c>) combined using a bitwise OR, then cast to an integer
    /// </param>
    /// <returns><c>true</c> if the button(s) is(are) held</returns>
    public bool GetButton(int mask)
    {
        return buffer[0].GetButton(mask);
    }

    /// <summary>
    /// The method for detecting if a button is currently held
    /// </summary>
    /// <param name="button">The button</param>
    /// <returns><c>true</c> if the button is held</returns>
    public bool GetButton(Buttons button)
    {
        return GetButton((int)button);
    }

    /// <summary>
    /// The method for detecting if one or more buttons have just been pressed
    /// </summary>
    /// <param name="mask">
    /// The button(s) as a mask. The mask can be created using elements of the button enum
    /// (<c>MVMXIV.Buttons</c>) combined using a bitwise OR, then cast to an integer
    /// </param>
    /// <returns><c>true</c> if the button got pressed or the buttons got pressed simultaneously</returns>
    public bool GetButtonDown(int mask)
    {
        return buffer[0].GetButton(mask) && !buffer[1].GetButton(mask);
    }

    /// <summary>
    /// The method for detecting if a buttons has just been pressed
    /// </summary>
    /// <param name="button">The button</param>
    /// <returns><c>true</c> if the button got pressed</returns>
    public bool GetButtonDown(Buttons button)
    {
        return GetButtonDown((int)button);
    }

    /// <summary>
    /// The method for detecting if one or more buttons have just been released
    /// </summary>
    /// <param name="mask">
    /// The button(s) as a mask. The mask can be created using elements of the button enum
    /// (<c>MVMXIV.Buttons</c>) combined using a bitwise OR, then cast to an integer
    /// </param>
    /// <returns><c>true</c> if the button got released or the buttons got released simultaneously</returns>
    public bool GetButtonUp(int mask)
    {
        return !buffer[0].GetButton(mask) && buffer[1].GetButton(mask);
    }

    /// <summary>
    /// The method for detecting if a button has just been released
    /// </summary>
    /// <param name="button">The button</param>
    /// <returns><c>true</c> if the button got released</returns>
    public bool GetButtonUp(Buttons button)
    {
        return GetButtonUp((int)button);
    }

    /// <summary>
    /// The method for detecting if a button has just been pressed AND released within the given amount of time.
    /// To check if multiple buttons were pressed, run this method on each button. Then, logical AND the result
    /// </summary>
    /// <param name="button">The button</param>
    /// <param name="deltaFrames">The number of frames within the input buffer to measure the change across</param>
    /// <returns><c>true</c> if the button got pressed AND released within the given number of input frames</returns>
    public bool GetButtonPressed(Buttons button, int deltaFrames)
    {
        int btn = (int)button;
        // Button must have released on the current input frame to count as a press
        if (buffer[0].GetButton(btn))
        {
            return false;
        }
        deltaFrames = AssertDeltaFrames(deltaFrames, 2);

        int i = 1;
        // We know it is not held, so check if it was
        for (; i <= deltaFrames; i++)
        {
            if (buffer[i].GetButton(btn))
            {
                // It was held, so we know that it was released
                break;
            }
        }
        // Now that we know it's released, we check if it was unheld
        for (; i <= deltaFrames; i++)
        {
            if (!buffer[i].GetButton(btn))
            {
                // It was initially unheld, so we know it was pressed
                break;
            }
        }

        // i == deltaFrames if we ran through the buffer without it being fully pressed
        return i < deltaFrames;
    }

    /// <summary>
    /// The method for detecting if a button has just been pressed AND released.
    /// To check if multiple buttons were pressed, run this method on each button. Then, logical AND the result
    /// </summary>
    /// <param name="button">The button</param>
    /// <returns><c>true</c> if the button got pressed AND released within the input buffer</returns>
    public bool GetButtonPressed(Buttons button)
    {
        return GetButtonPressed(button, buffer.Length - 1);
    }

    /// <summary>
    /// The method for detecting if a button has just been pressed AND released twice within the given amount of time.
    /// To check if multiple buttons were double pressed, run this method on each button. Then, logical AND the result
    /// </summary>
    /// <param name="button">The button</param>
    /// <param name="deltaFrames">The number of frames within the input buffer to measure the change across</param>
    /// <returns><c>true</c> if the button got pressed AND released twice within the given number of input frames</returns>
    public bool GetDoublePressed(Buttons button, int deltaFrames)
    {
        int btn = (int)button;
        // Button must have released on the current input frame to count as a press
        if (buffer[0].GetButton(btn))
        {
            return false;
        }
        deltaFrames = AssertDeltaFrames(deltaFrames, 4);

        int i = 1;
        // We know it is not held, so check if it was
        for (; i <= deltaFrames; i++)
        {
            if (buffer[i].GetButton(btn))
            {
                // It was held, so we know that it was released once
                break;
            }
        }
        // Now that we know it's released, we check if it was unheld
        for (; i <= deltaFrames; i++)
        {
            if (!buffer[i].GetButton(btn))
            {
                // It was initially unheld, so we know it was pressed once
                break;
            }
        }
        // Check if it was released a second time
        // (This is just the same two for-loops above repeated again)
        // TODO Make this pair of loops a local method since it's used 3 times?
        for (; i <= deltaFrames; i++)
        {
            if (buffer[i].GetButton(btn))
            {
                // It was held, so we know that it was released twice
                break;
            }
        }
        // Check if it was pressed a second time
        for (; i <= deltaFrames; i++)
        {
            if (!buffer[i].GetButton(btn))
            {
                // It was initially unheld, so we know it was pressed twice
                break;
            }
        }

        // i == deltaFrames if we ran through the buffer without it being fully, double pressed
        return i < deltaFrames;
    }

    /// <summary>
    /// The method for detecting if a button has just been pressed AND released twice
    /// To check if multiple buttons were double pressed, run this method on each button. Then, logical AND the result
    /// </summary>
    /// <param name="button">The button</param>
    /// <returns><c>true</c> if the button got pressed AND released twice within the input buffer</returns>
    public bool GetDoublePressed(Buttons button)
    {
        return GetDoublePressed(button, buffer.Length - 1);
    }

    int AssertDeltaFrames(int deltaFrames, int minFrame)
    {
        bool hasNoChange = deltaFrames < minFrame;
        bool isGreaterThanBufferSize = deltaFrames >= buffer.Length;
        Debug.Assert(!(hasNoChange && isGreaterThanBufferSize), $"{deltaFrames} delta-frames is outside the range of the input buffer!");
        if (hasNoChange)
        {
            deltaFrames = minFrame;
        }
        else if (isGreaterThanBufferSize)
        {
            deltaFrames = buffer.Length - 1;
        }

        return deltaFrames;
    }

    int AssertDeltaFrames(int deltaFrames)
    {
        return AssertDeltaFrames(deltaFrames, 1);
    }

    class InputFrame
    {
        float[] axis = new float[2];
        int buttons; // 32 button max

        public InputFrame()
        {
            axis[0] = Input.GetAxisRaw("Horizontal");
            axis[1] = Input.GetAxisRaw("Vertical");

            buttons = 0x0000;
            buttons |= ToInt(axis[0] > 0);
            buttons |= ToInt(axis[0] < 0) << 1;
            buttons |= ToInt(axis[1] > 0) << 2;
            buttons |= ToInt(axis[1] < 0) << 3;
            buttons |= ToInt(Input.GetButton("Fire1")) << 4;
            buttons |= ToInt(Input.GetButton("Fire2")) << 5;
            buttons |= ToInt(Input.GetButton("Fire3")) << 6;
            buttons |= ToInt(Input.GetButton("Jump")) << 7;
            buttons |= ToInt(Input.GetButton("Submit")) << 8;
            buttons |= ToInt(Input.GetButton("Cancel")) << 9;
        }

        public InputFrame(bool isInitialization)
        {
            if (!isInitialization)
            {
                return;
            }

            for (int i = 0; i < axis.Length; i++)
            {
                axis[i] = 0.0f;
            }
            buttons = 0x00000000;
        }

        public float GetAxis(int i)
        {
            bool isInvalid = i < 0 || i >= axis.Length;
            Debug.Assert(!isInvalid, $"{i} is not a valid input axis!");
            if (isInvalid)
            {
                return 0.0f;
            }

            return axis[i];
        }

        public bool GetButton(int mask)
        {
            return (mask & buttons) == mask;
        }

        int ToInt(bool b)
        {
            return b ? 0x00000001 : 0x00000000;
        }
    }
}
