import { Unity, useUnityContext } from "react-unity-webgl";

export default function UnityPlayer() {

    const { unityProvider } = useUnityContext({
        loaderUrl: "unity/Build/unity.loader.js",
        dataUrl: "unity/Build/unity.data",
        frameworkUrl: "unity/Build/unity.framework.js",
        codeUrl: "unity/Build/unity.wasm"
    });

    return (
        <div style={{ position: "relative", width: "100%", height: "100vh" }}>

            <Unity unityProvider={unityProvider} style={{ width: "100%", height: "100vh" }} />

            <div
                style={{
                    position: "absolute", top: 10, left: 10,
                    background: "#afd9ea",
                    padding: "10px",
                    border: "2px solid white",
                    borderRadius: "5px",
                    boxShadow: "0 2px 10px rgba(0,0,0,0.3)",
                    zIndex: 10
                }}
            >
                <p>
                    Avancer: Z<br/>
                    Reculer: S<br/>
                    Aller à gauche: Q<br/>
                    Aller à droite: D<br/>
                    Monter: A<br/>
                    Descendre: E<br/>
                    Bouger l'angle de la caméra: Bouton droit de la souris
                </p>
            </div>

        </div>
    );
}