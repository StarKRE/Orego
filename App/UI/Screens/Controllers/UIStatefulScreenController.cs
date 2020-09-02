using System;
using System.Collections.Generic;
using System.Linq;
using OregoFramework.Unit;

namespace OregoFramework.App
{
    public abstract class UIStatefulScreenController : UICachedScreenController
    {
        #region Const

        private readonly Type statefulUiType = typeof(IStatefulUI);

        #endregion

        private readonly Dictionary<Type, IUIStateAdapter> screenTypeVsStateAdapterMap;

        protected UIStatefulScreenController()
        {
            this.screenTypeVsStateAdapterMap = new Dictionary<Type, IUIStateAdapter>();
        }

        #region Initialize

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var screenTypes = this.screenTypeVsResourceMap.Keys;
            var statefulScreenTypes = screenTypes.Where(type =>
                statefulUiType.IsAssignableFrom(type));
            foreach (var statefulScreenType in statefulScreenTypes)
            {
                var stateAdapter = this.LoadStateAdapter(statefulScreenType);
                this.screenTypeVsStateAdapterMap[statefulScreenType] = stateAdapter;
            }
        }

        protected abstract IUIStateAdapter LoadStateAdapter(Type screenType);

        #endregion

        protected override UIScreen LoadScreen(Type screenType)
        {
            var screen = base.LoadScreen(screenType);
            if (screen is IStatefulUI statefulScreen)
            {
                var stateProvider = this.screenTypeVsStateAdapterMap[screenType];
                var state = stateProvider.Get();
                statefulScreen.OnEnterState(state);
            }

            return screen;
        }

        protected override void UnloadScreen(UIScreen screen)
        {
            if (screen is IStatefulUI statefulScreen)
            {
                var screenType = screen.GetType();
                var state = statefulScreen.OnExitState();
                var stateProvider = this.screenTypeVsStateAdapterMap[screenType];
                stateProvider.Set(state);
            }

            base.UnloadScreen(screen);
        }
    }
}