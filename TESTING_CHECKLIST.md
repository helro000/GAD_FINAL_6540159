# Unity manual testing checklist

The code can be inspected as files, but runtime tests must be run inside Unity. After following the README, verify:

- [ ] No red compile errors in Unity Console.
- [ ] Scene `MainMenu` opens when you press Play.
- [ ] Main menu looks correct and has **four** buttons.
- [ ] `Mad Driver` opens the Driving scene; W/S drives; A/D steers; crossing finish shows victory.
- [ ] `Fly Like a Bird` opens the Flying scene; use A/D and W/S to pass through golden rings.
- [ ] `I'm a Sumo and a Ball` opens the Sumo scene; WASD moves and Space dashes.
- [ ] **Exit** stops Play mode (or closes the standalone app).
- [ ] Pressing **Escape** in each game freezes gameplay and shows the **PAUSED** overlay.
- [ ] **Resume** hides the overlay and unfreezes gameplay.
- [ ] **Restart** resets all positions, score and obstacles.
- [ ] **Back to Main Menu** returns to the main screen.
- [ ] Win/loss overlays offer **PLAY AGAIN** and **MAIN MENU**.
- [ ] Game runs in a standalone build with scene `MainMenu` first.
- [ ] All work and screen recording comply with the instructor's exam rules and timing.

If controls do not work, set the project's Active Input Handling to **Both** or **Input Manager (Old)** as described in README.
