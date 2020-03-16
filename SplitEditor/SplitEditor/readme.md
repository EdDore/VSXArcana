# Split Editor Sample Code

This is a proof of concept VS SDK Package illustrating how to create a multi-view editor that hosts a custom designer along with the VS code editor, using a WinForm SplitContainer control.

The bulk of the functionality for creating/hosting the "child" editors is contained withing the SplitViewEditorPane class and in the CreateChildEditorFrame method, where we set a number of IVsWindowFrame properties to properly parent the child editor within the respective SplitContainer control panes.


