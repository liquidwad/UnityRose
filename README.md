# UnityRose
Unity version of Rose Online
<h2>Project setup</h2>
<p>
  <ul>
    <li>Open a terminal and cd to a desired location and clone this repo: </li>
    <li><code>git clone https://github.com/liquidwad/UnityRose.git</code></li>
    <li>Open unity, open project, and choose the directory that was just cloned.</li>
  </ul>
</p>

<h2>Asset Setup</h2>
<p>
  <ul>
    <li>Obtain a copy of Rose Online's extracted 3DDATA.</li>
    <li>Convert all textures to .png using <a href="http://developer.amd.com/tools-and-sdks/archive/legacy-cpu-gpu-tools/the-compressonator/" target="_blank">the Compressonator</a></li>
    <li>Create a Unity project and select the desired target build platform (assets need to be re-imported every time you change the target<li>
    <li>Copy 3DATA to the Unity Project's Assets folder. Open Unity and let it import. This will take a LONG TIME.</li>
    <li>In unity Project explorer: Search by texture and find all the textures under 3DDATA/TERRAIN</li>
    <li>Select all the textures and in the inspector choose Advanced for textured type, then check R/W enable and uncheck generate mipmaps</li>
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
    <li>In Unity: GameObject->Create Other->Rose Object.  Click Generate Animations</li>
    <li>After about half an hour, check Resources folder to make sure Animation resources were generated</code></li>
  </ul>
</p>

<h2>Server usage</h2>
<p>
  <ul>
    <li>Download & Install <a href="http://www.mongodb.org/" target="_blank">MongoDB</a></li>
    <li>Download & Install <a href="http://nodejs.org/download/" target="_blank">Node.js</a></li>
    <li>Run <code>npm install</code> in the server folder to download all packages</li>
    <li>Start Mongodb instance - <a href="http://docs.mongodb.org/manual/" target="_blank">Manual</a></li>
    <li>Run <code>npm start</code> or <code>nodemon server.js</code> in the server folder</li>
    <li>To install a database admin tool, follow insturctions in <a href="https://www.npmjs.com/package/mongo-express" target="_blank">npm-express</a></li>
  </ul>
</p>
