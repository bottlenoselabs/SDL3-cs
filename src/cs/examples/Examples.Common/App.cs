// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using bottlenoselabs.SDL;

namespace Common;

public sealed class App : Application
{
    private int _exampleIndex = -1;
    private int _goToExampleIndex;
    private int _examplesCount;
    private ImmutableArray<Type> _exampleTypes = [];
    private ExampleBase? _currentExample;
    private readonly ArenaNativeAllocator _allocator = new(1024);

    protected override void OnStart()
    {
        var assemblyName = Assembly.GetEntryAssembly()!.GetName().Name;
        Console.WriteLine($"Welcome to the {assemblyName} suite!");
        Console.WriteLine("Press 1/2 (or LB/RB) to move between examples!");

        _exampleTypes = [
            ..AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(ExampleBase).IsAssignableFrom(type) && !type.IsAbstract)
        ];
        _examplesCount = _exampleTypes.Length;
    }

    protected override void OnExit()
    {
        _currentExample?.Exit();
        _currentExample = null;
    }

    protected override void OnMouseMove(in MouseMoveEvent e)
    {
        _currentExample?.OnMouseMove(e);
    }

    protected override void OnMouseDown(in MouseButtonEvent e)
    {
        _currentExample?.OnMouseDown(e);
    }

    protected override void OnMouseUp(in MouseButtonEvent e)
    {
        _currentExample?.OnMouseUp(e);
    }

    protected override void OnKeyDown(in KeyboardEvent e)
    {
        if (e.Button == KeyboardButton.Number2)
        {
            _goToExampleIndex = _exampleIndex + 1;
            if (_goToExampleIndex >= _examplesCount)
            {
                _goToExampleIndex = 0;
            }
        }
        else if (e.Button == KeyboardButton.Number1)
        {
            _goToExampleIndex = _exampleIndex - 1;
            if (_goToExampleIndex < 0)
            {
                _goToExampleIndex = _examplesCount - 1;
            }
        }

        _currentExample?.OnKeyDown(e);
    }

    protected override void OnKeyUp(in KeyboardEvent e)
    {
        _currentExample?.OnKeyUp(e);
    }

    protected override void OnUpdate(TimeSpan deltaTime)
    {
        if (_goToExampleIndex != -1)
        {
            var previousExample = _currentExample;
            if (previousExample != null)
            {
                _currentExample = null;
                previousExample.Exit();
                // ReSharper disable once RedundantAssignment
#pragma warning disable IDE0059
                previousExample = null;
#pragma warning restore IDE0059

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                var bytesEnding = Process.GetCurrentProcess().WorkingSet64;
                var bytesEndingString =
                    (bytesEnding / Math.Pow(1024, 2)).ToString("0.00 MB", CultureInfo.InvariantCulture);
                Console.WriteLine("ENDING EXAMPLE, TOTAL MEMORY SIZE AFTER QUIT: {0}", bytesEndingString);
            }

            _exampleIndex = _goToExampleIndex;
            _currentExample = (ExampleBase)Activator.CreateInstance(_exampleTypes[_exampleIndex])!;
            var bytesStartingBeforeInit = Process.GetCurrentProcess().WorkingSet64;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            var bytesStartingStringBeforeInit =
                (bytesStartingBeforeInit / Math.Pow(1024, 2)).ToString("0.00 MB", CultureInfo.InvariantCulture);
            Console.WriteLine("STARTING EXAMPLE: '{0}', TOTAL MEMORY SIZE BEFORE INIT: {1}", _currentExample.Name, bytesStartingStringBeforeInit);

            var isSuccessfullyStarted = _currentExample.Start(_allocator);
            if (!isSuccessfullyStarted)
            {
                Console.Error.WriteLine("\nStarting example failed!");
                _currentExample = null;
                Exit();
                return;
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            var bytesStartingAfterInit = Process.GetCurrentProcess().WorkingSet64;
            var bytesStartingStringAfterInit =
                (bytesStartingAfterInit / Math.Pow(1024, 2)).ToString("0.00 MB", CultureInfo.InvariantCulture);
            Console.WriteLine("INITIALIZED EXAMPLE: '{0}', TOTAL MEMORY SIZE AFTER INIT: {1}", _currentExample.Name, bytesStartingStringAfterInit);

            _goToExampleIndex = -1;
        }

        if (_currentExample != null)
        {
            _currentExample.Window.Title = _currentExample.Name + ", FPS: " + FramesPerSecond;
            _currentExample.OnUpdate(deltaTime);
        }
    }

    protected override void OnDraw(TimeSpan deltaTime)
    {
        _currentExample?.OnDraw(deltaTime);
    }
}
