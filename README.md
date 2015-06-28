# UnityRose
Unity version of Rose Online

<h2>Server usage</h2>
<p>
  <ul>
    <li>Download & Install <a href="http://www.mongodb.org/" target="_blank">MongoDB</a></li>
    <li>Download & Install <a href="http://nodejs.org/download/" target="_blank">Node.js</a></li>
    <li>Run <code>npm install</code> in the server folder to download all packages</li>
    <li>Start Mongodb instance - <a href="http://docs.mongodb.org/manual/" target="_blank">Manual</a></li>
    <li>Run <code>npm start</code> or <code>nodemon server.js</code> in the server folder</li>
  </ul>
</p>

<h2>Asset Setup</h2>
<p>
  <ul>
    <li>Obtain a copy of Rose Online's extracted 3DDATA and put it under Assets folder</li>
    <li>Create this directory: {UnityProjectPath}/Assets/Resources/3DDATA</li>
    <li>Copy 3DDATA/AVATAR to .../Assets/Resources/3DDATA</li>
    <li>Copy 3DDATA/STB to .../Assets/Resources/3DDATA</li>
    <li>Open command prompt / terminal and cd to .../Assets/Resources/3DDATA</li>
    <li>On Windows: </li>
    <p>
      <li><code>for /R %x in (*.zms) do ren "%x" *.zms.bytes</code></li>
      <li><code>for /R %x in (*.zmd) do ren "%x" *.zmd.bytes</code></li>
      <li><code>for /R %x in (*.zmo) do ren "%x" *.zmo.bytes</code></li>
      <li><code>for /R %x in (*.zsc) do ren "%x" *.zsc.bytes</code></li>
      <li><code>for /R %x in (*.stb) do ren "%x" *.stb.bytes</code></li>
      <li><code>for /R %x in (*.stl) do ren "%x" *.stl.bytes</code></li>
    </p>
    <li>On Mac: </li>
    <p>
      <li><code>find . -name "*.ZMS" -exec rename -v 's/\.ZMS$/\.ZMS.bytes/i' {} \;</code></li>
      <li><code>find . -name "*.ZMD" -exec rename -v 's/\.ZMD$/\.ZMD.bytes/i' {} \;</code></li>
      <li><code>find . -name "*.ZMO" -exec rename -v 's/\.ZMO$/\.ZMO.bytes/i' {} \;</code></li>
      <li><code>find . -name "*.ZSC" -exec rename -v 's/\.ZSC$/\.ZSC.bytes/i' {} \;</code></li>
      <li><code>find . -name "*.STB" -exec rename -v 's/\.STB$/\.STB.bytes/i' {} \;</code></li>
      <li><code>find . -name "*.STL" -exec rename -v 's/\.ZSC$/\.STL.bytes/i' {} \;</code></li>
    </p>
    <li>Generate animation prefabs by calling         <code>UnityRose.ResourceManager.Instance.GenerateAnimationAssets();</code></li>
    <li>Check Resources folder to make sure Animation resources were generated</code></li>
  </ul>
</p>
