# SNAPI (Snake API)
  Both an API for Developers, and a Plugin for Server Hosts!

  Features EXILED Based Events for Actions Players make like:
  ```
  OnGameOver
  OnPausingSnake
  OnResumingSnake
  OnScore
  OnSnakeMove
  OnStartingNewSnake
  OnSwitchAxes
  ```

  While also featuring Configs like
  ```
  AllowSnake
  MaxPlayTime
  CooldownTime
  ```
  And More!

  **Please contact ```atsomeone``` on Discord for any questions or concerns!**


# Multiple Versions
  SNAPI Has 3 Versions. Exiled, HSM, and RueI. Each of these correlate to a different Hint management system. If your server has HSM, use the HSM build, etc.
  This is useful for some of the configs for Server Hosts as they can control which Hint framework they want simply based on version.


# How It Works
  Snake in SCP:SL (14.1 Public Beta) is client side and cannot be controlled by a server. How then did I accomplish what I did?
  
  I used the SnakeNetworkMessage that clients send to the Server to synchronize with other clients on the server. With these data packets, I can infer all details about a snake game currently being played. (Do keep this in mind developers)
  All I need to add then is functionality to force-unequip their keycard and I would have a complete plugin. However, SNAPI also adds all the relevant Events you would need if you wanted to base a plugin off this data as well.
  Because this is a plugin however, some internal fields are kept internal in certain classes like SnakeContext. If you need these properties avaliable to you, don't hesitate to fork this repo and adjust the plugin to your liking.

  Thanks for Reading!
