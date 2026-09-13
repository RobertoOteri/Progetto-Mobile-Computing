extern "C" void RegisterStaticallyLinkedModulesGranular()
{
	void RegisterModule_SharedInternals();
	RegisterModule_SharedInternals();

	void RegisterModule_Core();
	RegisterModule_Core();

	void RegisterModule_Animation();
	RegisterModule_Animation();

	void RegisterModule_Audio();
	RegisterModule_Audio();

	void RegisterModule_ContentLoad();
	RegisterModule_ContentLoad();

	void RegisterModule_GameCenter();
	RegisterModule_GameCenter();

	void RegisterModule_GraphicsStateCollectionSerializer();
	RegisterModule_GraphicsStateCollectionSerializer();

	void RegisterModule_Grid();
	RegisterModule_Grid();

	void RegisterModule_HierarchyCore();
	RegisterModule_HierarchyCore();

	void RegisterModule_IMGUI();
	RegisterModule_IMGUI();

	void RegisterModule_ImageConversion();
	RegisterModule_ImageConversion();

	void RegisterModule_InputLegacy();
	RegisterModule_InputLegacy();

	void RegisterModule_InputForUI();
	RegisterModule_InputForUI();

	void RegisterModule_JSONSerialize();
	RegisterModule_JSONSerialize();

	void RegisterModule_Mathematics();
	RegisterModule_Mathematics();

	void RegisterModule_Physics();
	RegisterModule_Physics();

	void RegisterModule_Scripting();
	RegisterModule_Scripting();

	void RegisterModule_PhysicsBackendPhysX();
	RegisterModule_PhysicsBackendPhysX();

	void RegisterModule_PhysicsCore2D();
	RegisterModule_PhysicsCore2D();

	void RegisterModule_Physics2D();
	RegisterModule_Physics2D();

	void RegisterModule_Properties();
	RegisterModule_Properties();

	void RegisterModule_RuntimeInitializeOnLoadManagerInitializer();
	RegisterModule_RuntimeInitializeOnLoadManagerInitializer();

	void RegisterModule_SpriteShape();
	RegisterModule_SpriteShape();

	void RegisterModule_TextRendering();
	RegisterModule_TextRendering();

	void RegisterModule_TextCoreFontEngine();
	RegisterModule_TextCoreFontEngine();

	void RegisterModule_TextCoreTextEngine();
	RegisterModule_TextCoreTextEngine();

	void RegisterModule_Tilemap();
	RegisterModule_Tilemap();

	void RegisterModule_U2DRuntime();
	RegisterModule_U2DRuntime();

	void RegisterModule_UI();
	RegisterModule_UI();

	void RegisterModule_UIElements();
	RegisterModule_UIElements();

}

template <typename T> void RegisterUnityClass(const char*);
template <typename T> void RegisterStrippedType(int, const char*, const char*);

void InvokeRegisterStaticallyLinkedModuleClasses()
{
	// Do nothing (we're in stripping mode)
}

class AnimationClip; template <> void RegisterUnityClass<AnimationClip>(const char*);
class Animator; template <> void RegisterUnityClass<Animator>(const char*);
class AnimatorController; template <> void RegisterUnityClass<AnimatorController>(const char*);
class AnimatorOverrideController; template <> void RegisterUnityClass<AnimatorOverrideController>(const char*);
class Motion; template <> void RegisterUnityClass<Motion>(const char*);
class RuntimeAnimatorController; template <> void RegisterUnityClass<RuntimeAnimatorController>(const char*);
class AudioBehaviour; template <> void RegisterUnityClass<AudioBehaviour>(const char*);
class AudioClip; template <> void RegisterUnityClass<AudioClip>(const char*);
class AudioListener; template <> void RegisterUnityClass<AudioListener>(const char*);
class AudioManager; template <> void RegisterUnityClass<AudioManager>(const char*);
class AudioMixer; template <> void RegisterUnityClass<AudioMixer>(const char*);
class AudioMixerGroup; template <> void RegisterUnityClass<AudioMixerGroup>(const char*);
class AudioMixerSnapshot; template <> void RegisterUnityClass<AudioMixerSnapshot>(const char*);
class AudioResource; template <> void RegisterUnityClass<AudioResource>(const char*);
class AudioSource; template <> void RegisterUnityClass<AudioSource>(const char*);
class SampleClip; template <> void RegisterUnityClass<SampleClip>(const char*);
class Behaviour; template <> void RegisterUnityClass<Behaviour>(const char*);
class BuildSettings; template <> void RegisterUnityClass<BuildSettings>(const char*);
class Camera; template <> void RegisterUnityClass<Camera>(const char*);
namespace Unity { class Component; } template <> void RegisterUnityClass<Unity::Component>(const char*);
class ComputeShader; template <> void RegisterUnityClass<ComputeShader>(const char*);
class Cubemap; template <> void RegisterUnityClass<Cubemap>(const char*);
class CubemapArray; template <> void RegisterUnityClass<CubemapArray>(const char*);
class DelayedCallManager; template <> void RegisterUnityClass<DelayedCallManager>(const char*);
class EditorExtension; template <> void RegisterUnityClass<EditorExtension>(const char*);
class GameManager; template <> void RegisterUnityClass<GameManager>(const char*);
class GameObject; template <> void RegisterUnityClass<GameObject>(const char*);
class GlobalGameManager; template <> void RegisterUnityClass<GlobalGameManager>(const char*);
class GraphicsSettings; template <> void RegisterUnityClass<GraphicsSettings>(const char*);
class InputManager; template <> void RegisterUnityClass<InputManager>(const char*);
class LevelGameManager; template <> void RegisterUnityClass<LevelGameManager>(const char*);
class Light; template <> void RegisterUnityClass<Light>(const char*);
class LightProbes; template <> void RegisterUnityClass<LightProbes>(const char*);
class LightingSettings; template <> void RegisterUnityClass<LightingSettings>(const char*);
class LightmapSettings; template <> void RegisterUnityClass<LightmapSettings>(const char*);
class LowerResBlitTexture; template <> void RegisterUnityClass<LowerResBlitTexture>(const char*);
class Material; template <> void RegisterUnityClass<Material>(const char*);
class Mesh; template <> void RegisterUnityClass<Mesh>(const char*);
class MeshFilter; template <> void RegisterUnityClass<MeshFilter>(const char*);
class MeshRenderer; template <> void RegisterUnityClass<MeshRenderer>(const char*);
class MonoBehaviour; template <> void RegisterUnityClass<MonoBehaviour>(const char*);
class MonoManager; template <> void RegisterUnityClass<MonoManager>(const char*);
class MonoScript; template <> void RegisterUnityClass<MonoScript>(const char*);
class NamedObject; template <> void RegisterUnityClass<NamedObject>(const char*);
class Object; template <> void RegisterUnityClass<Object>(const char*);
class PlayerSettings; template <> void RegisterUnityClass<PlayerSettings>(const char*);
class PreloadData; template <> void RegisterUnityClass<PreloadData>(const char*);
class QualitySettings; template <> void RegisterUnityClass<QualitySettings>(const char*);
namespace UI { class RectTransform; } template <> void RegisterUnityClass<UI::RectTransform>(const char*);
class ReflectionProbe; template <> void RegisterUnityClass<ReflectionProbe>(const char*);
class RenderSettings; template <> void RegisterUnityClass<RenderSettings>(const char*);
class RenderTexture; template <> void RegisterUnityClass<RenderTexture>(const char*);
class Renderer; template <> void RegisterUnityClass<Renderer>(const char*);
class ResourceManager; template <> void RegisterUnityClass<ResourceManager>(const char*);
class RuntimeInitializeOnLoadManager; template <> void RegisterUnityClass<RuntimeInitializeOnLoadManager>(const char*);
class Shader; template <> void RegisterUnityClass<Shader>(const char*);
class ShaderNameRegistry; template <> void RegisterUnityClass<ShaderNameRegistry>(const char*);
class SortingGroup; template <> void RegisterUnityClass<SortingGroup>(const char*);
class Sprite; template <> void RegisterUnityClass<Sprite>(const char*);
class SpriteAtlas; template <> void RegisterUnityClass<SpriteAtlas>(const char*);
class SpriteRenderer; template <> void RegisterUnityClass<SpriteRenderer>(const char*);
class TagManager; template <> void RegisterUnityClass<TagManager>(const char*);
class TextAsset; template <> void RegisterUnityClass<TextAsset>(const char*);
class Texture; template <> void RegisterUnityClass<Texture>(const char*);
class Texture2D; template <> void RegisterUnityClass<Texture2D>(const char*);
class Texture2DArray; template <> void RegisterUnityClass<Texture2DArray>(const char*);
class Texture3D; template <> void RegisterUnityClass<Texture3D>(const char*);
class TimeManager; template <> void RegisterUnityClass<TimeManager>(const char*);
class Transform; template <> void RegisterUnityClass<Transform>(const char*);
class Grid; template <> void RegisterUnityClass<Grid>(const char*);
class GridLayout; template <> void RegisterUnityClass<GridLayout>(const char*);
class BoxCollider; template <> void RegisterUnityClass<BoxCollider>(const char*);
class CapsuleCollider; template <> void RegisterUnityClass<CapsuleCollider>(const char*);
class Collider; template <> void RegisterUnityClass<Collider>(const char*);
class MeshCollider; template <> void RegisterUnityClass<MeshCollider>(const char*);
class PhysicsManager; template <> void RegisterUnityClass<PhysicsManager>(const char*);
class Rigidbody; template <> void RegisterUnityClass<Rigidbody>(const char*);
class SphereCollider; template <> void RegisterUnityClass<SphereCollider>(const char*);
class BoxCollider2D; template <> void RegisterUnityClass<BoxCollider2D>(const char*);
class CapsuleCollider2D; template <> void RegisterUnityClass<CapsuleCollider2D>(const char*);
class CircleCollider2D; template <> void RegisterUnityClass<CircleCollider2D>(const char*);
class Collider2D; template <> void RegisterUnityClass<Collider2D>(const char*);
class CompositeCollider2D; template <> void RegisterUnityClass<CompositeCollider2D>(const char*);
class EdgeCollider2D; template <> void RegisterUnityClass<EdgeCollider2D>(const char*);
class Physics2DSettings; template <> void RegisterUnityClass<Physics2DSettings>(const char*);
class PolygonCollider2D; template <> void RegisterUnityClass<PolygonCollider2D>(const char*);
class Rigidbody2D; template <> void RegisterUnityClass<Rigidbody2D>(const char*);
class PhysicsCoreProjectSettings2D; template <> void RegisterUnityClass<PhysicsCoreProjectSettings2D>(const char*);
class SpriteShapeRenderer; template <> void RegisterUnityClass<SpriteShapeRenderer>(const char*);
namespace TextRendering { class Font; } template <> void RegisterUnityClass<TextRendering::Font>(const char*);
namespace TextRenderingPrivate { class TextMesh; } template <> void RegisterUnityClass<TextRenderingPrivate::TextMesh>(const char*);
class Tilemap; template <> void RegisterUnityClass<Tilemap>(const char*);
class TilemapCollider2D; template <> void RegisterUnityClass<TilemapCollider2D>(const char*);
class TilemapRenderer; template <> void RegisterUnityClass<TilemapRenderer>(const char*);
namespace UI { class Canvas; } template <> void RegisterUnityClass<UI::Canvas>(const char*);
namespace UI { class CanvasGroup; } template <> void RegisterUnityClass<UI::CanvasGroup>(const char*);
namespace UI { class CanvasRenderer; } template <> void RegisterUnityClass<UI::CanvasRenderer>(const char*);
class PanelRenderer; template <> void RegisterUnityClass<PanelRenderer>(const char*);
class UIAnimationBinder; template <> void RegisterUnityClass<UIAnimationBinder>(const char*);
class UIRenderer; template <> void RegisterUnityClass<UIRenderer>(const char*);

void RegisterAllClasses()
{
void RegisterBuiltinTypes();
RegisterBuiltinTypes();
	//Total: 100 non stripped classes
	//0. AnimationClip
	RegisterUnityClass<AnimationClip>("Animation");
	//1. Animator
	RegisterUnityClass<Animator>("Animation");
	//2. AnimatorController
	RegisterUnityClass<AnimatorController>("Animation");
	//3. AnimatorOverrideController
	RegisterUnityClass<AnimatorOverrideController>("Animation");
	//4. Motion
	RegisterUnityClass<Motion>("Animation");
	//5. RuntimeAnimatorController
	RegisterUnityClass<RuntimeAnimatorController>("Animation");
	//6. AudioBehaviour
	RegisterUnityClass<AudioBehaviour>("Audio");
	//7. AudioClip
	RegisterUnityClass<AudioClip>("Audio");
	//8. AudioListener
	RegisterUnityClass<AudioListener>("Audio");
	//9. AudioManager
	RegisterUnityClass<AudioManager>("Audio");
	//10. AudioMixer
	RegisterUnityClass<AudioMixer>("Audio");
	//11. AudioMixerGroup
	RegisterUnityClass<AudioMixerGroup>("Audio");
	//12. AudioMixerSnapshot
	RegisterUnityClass<AudioMixerSnapshot>("Audio");
	//13. AudioResource
	RegisterUnityClass<AudioResource>("Audio");
	//14. AudioSource
	RegisterUnityClass<AudioSource>("Audio");
	//15. SampleClip
	RegisterUnityClass<SampleClip>("Audio");
	//16. Behaviour
	RegisterUnityClass<Behaviour>("Core");
	//17. BuildSettings
	RegisterUnityClass<BuildSettings>("Core");
	//18. Camera
	RegisterUnityClass<Camera>("Core");
	//19. Component
	RegisterUnityClass<Unity::Component>("Core");
	//20. ComputeShader
	RegisterUnityClass<ComputeShader>("Core");
	//21. Cubemap
	RegisterUnityClass<Cubemap>("Core");
	//22. CubemapArray
	RegisterUnityClass<CubemapArray>("Core");
	//23. DelayedCallManager
	RegisterUnityClass<DelayedCallManager>("Core");
	//24. EditorExtension
	RegisterUnityClass<EditorExtension>("Core");
	//25. GameManager
	RegisterUnityClass<GameManager>("Core");
	//26. GameObject
	RegisterUnityClass<GameObject>("Core");
	//27. GlobalGameManager
	RegisterUnityClass<GlobalGameManager>("Core");
	//28. GraphicsSettings
	RegisterUnityClass<GraphicsSettings>("Core");
	//29. InputManager
	RegisterUnityClass<InputManager>("Core");
	//30. LevelGameManager
	RegisterUnityClass<LevelGameManager>("Core");
	//31. Light
	RegisterUnityClass<Light>("Core");
	//32. LightProbes
	RegisterUnityClass<LightProbes>("Core");
	//33. LightingSettings
	RegisterUnityClass<LightingSettings>("Core");
	//34. LightmapSettings
	RegisterUnityClass<LightmapSettings>("Core");
	//35. LowerResBlitTexture
	RegisterUnityClass<LowerResBlitTexture>("Core");
	//36. Material
	RegisterUnityClass<Material>("Core");
	//37. Mesh
	RegisterUnityClass<Mesh>("Core");
	//38. MeshFilter
	RegisterUnityClass<MeshFilter>("Core");
	//39. MeshRenderer
	RegisterUnityClass<MeshRenderer>("Core");
	//40. MonoBehaviour
	RegisterUnityClass<MonoBehaviour>("Core");
	//41. MonoManager
	RegisterUnityClass<MonoManager>("Core");
	//42. MonoScript
	RegisterUnityClass<MonoScript>("Core");
	//43. NamedObject
	RegisterUnityClass<NamedObject>("Core");
	//44. Object
	//Skipping Object
	//45. PlayerSettings
	RegisterUnityClass<PlayerSettings>("Core");
	//46. PreloadData
	RegisterUnityClass<PreloadData>("Core");
	//47. QualitySettings
	RegisterUnityClass<QualitySettings>("Core");
	//48. RectTransform
	RegisterUnityClass<UI::RectTransform>("Core");
	//49. ReflectionProbe
	RegisterUnityClass<ReflectionProbe>("Core");
	//50. RenderSettings
	RegisterUnityClass<RenderSettings>("Core");
	//51. RenderTexture
	RegisterUnityClass<RenderTexture>("Core");
	//52. Renderer
	RegisterUnityClass<Renderer>("Core");
	//53. ResourceManager
	RegisterUnityClass<ResourceManager>("Core");
	//54. RuntimeInitializeOnLoadManager
	RegisterUnityClass<RuntimeInitializeOnLoadManager>("Core");
	//55. Shader
	RegisterUnityClass<Shader>("Core");
	//56. ShaderNameRegistry
	RegisterUnityClass<ShaderNameRegistry>("Core");
	//57. SortingGroup
	RegisterUnityClass<SortingGroup>("Core");
	//58. Sprite
	RegisterUnityClass<Sprite>("Core");
	//59. SpriteAtlas
	RegisterUnityClass<SpriteAtlas>("Core");
	//60. SpriteRenderer
	RegisterUnityClass<SpriteRenderer>("Core");
	//61. TagManager
	RegisterUnityClass<TagManager>("Core");
	//62. TextAsset
	RegisterUnityClass<TextAsset>("Core");
	//63. Texture
	RegisterUnityClass<Texture>("Core");
	//64. Texture2D
	RegisterUnityClass<Texture2D>("Core");
	//65. Texture2DArray
	RegisterUnityClass<Texture2DArray>("Core");
	//66. Texture3D
	RegisterUnityClass<Texture3D>("Core");
	//67. TimeManager
	RegisterUnityClass<TimeManager>("Core");
	//68. Transform
	RegisterUnityClass<Transform>("Core");
	//69. Grid
	RegisterUnityClass<Grid>("Grid");
	//70. GridLayout
	RegisterUnityClass<GridLayout>("Grid");
	//71. BoxCollider
	RegisterUnityClass<BoxCollider>("Physics");
	//72. CapsuleCollider
	RegisterUnityClass<CapsuleCollider>("Physics");
	//73. Collider
	RegisterUnityClass<Collider>("Physics");
	//74. MeshCollider
	RegisterUnityClass<MeshCollider>("Physics");
	//75. PhysicsManager
	RegisterUnityClass<PhysicsManager>("Physics");
	//76. Rigidbody
	RegisterUnityClass<Rigidbody>("Physics");
	//77. SphereCollider
	RegisterUnityClass<SphereCollider>("Physics");
	//78. BoxCollider2D
	RegisterUnityClass<BoxCollider2D>("Physics2D");
	//79. CapsuleCollider2D
	RegisterUnityClass<CapsuleCollider2D>("Physics2D");
	//80. CircleCollider2D
	RegisterUnityClass<CircleCollider2D>("Physics2D");
	//81. Collider2D
	RegisterUnityClass<Collider2D>("Physics2D");
	//82. CompositeCollider2D
	RegisterUnityClass<CompositeCollider2D>("Physics2D");
	//83. EdgeCollider2D
	RegisterUnityClass<EdgeCollider2D>("Physics2D");
	//84. Physics2DSettings
	RegisterUnityClass<Physics2DSettings>("Physics2D");
	//85. PolygonCollider2D
	RegisterUnityClass<PolygonCollider2D>("Physics2D");
	//86. Rigidbody2D
	RegisterUnityClass<Rigidbody2D>("Physics2D");
	//87. PhysicsCoreProjectSettings2D
	RegisterUnityClass<PhysicsCoreProjectSettings2D>("PhysicsCore2D");
	//88. SpriteShapeRenderer
	RegisterUnityClass<SpriteShapeRenderer>("SpriteShape");
	//89. Font
	RegisterUnityClass<TextRendering::Font>("TextRendering");
	//90. TextMesh
	RegisterUnityClass<TextRenderingPrivate::TextMesh>("TextRendering");
	//91. Tilemap
	RegisterUnityClass<Tilemap>("Tilemap");
	//92. TilemapCollider2D
	RegisterUnityClass<TilemapCollider2D>("Tilemap");
	//93. TilemapRenderer
	RegisterUnityClass<TilemapRenderer>("Tilemap");
	//94. Canvas
	RegisterUnityClass<UI::Canvas>("UI");
	//95. CanvasGroup
	RegisterUnityClass<UI::CanvasGroup>("UI");
	//96. CanvasRenderer
	RegisterUnityClass<UI::CanvasRenderer>("UI");
	//97. PanelRenderer
	RegisterUnityClass<PanelRenderer>("UIElements");
	//98. UIAnimationBinder
	RegisterUnityClass<UIAnimationBinder>("UIElements");
	//99. UIRenderer
	RegisterUnityClass<UIRenderer>("UIElements");

}
