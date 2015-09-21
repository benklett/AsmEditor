using System;
using System.IO;
using Gtk;
using Mono.TextEditor;

public partial class MainWindow: Gtk.Window
{
    private TextEditor editor;

    private bool isEdited;
    private string filename = null;

    public MainWindow()
        : base(Gtk.WindowType.Toplevel)
    {
        Build();

        editor = new TextEditor();
        editor.Document.LineChanged += (sender, e) => isEdited = true;;
        vbox1.Add(editor);
        vbox1.ReorderChild(editor, 1);

        ShowAll();
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    private void switchTextDocument()
    {
        if (filename != null)
            editor.Document.Text = File.ReadAllText(filename);
        else
            editor.Document.Text = "";
    }

    #region Events
    protected void compileAction_Activated(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    protected void compileSaveAsAction_Activated(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    protected void newAction_Activated(object sender, EventArgs e)
    {
        if (filename != null || isEdited)
        {
            MessageDialog md = new MessageDialog(
                                   this,
                                   DialogFlags.Modal | DialogFlags.DestroyWithParent,
                                   MessageType.Warning,
                                   ButtonsType.OkCancel,
                                   "Do you want to discard the currently opened file?");

            ResponseType resp = (ResponseType)md.Run();

            md.Destroy();

            if (resp == ResponseType.Cancel)
                return;
        }

        filename = null;
        switchTextDocument();
    }

    protected void openAction_Activated(object sender, EventArgs e)
    {
        if (filename != null || isEdited)
        {
            MessageDialog md = new MessageDialog(
                this,
                DialogFlags.Modal | DialogFlags.DestroyWithParent,
                MessageType.Warning,
                ButtonsType.OkCancel,
                "Do you want to discard the currently opened file?");

            ResponseType resp = (ResponseType)md.Run();

            md.Destroy();

            if (resp == ResponseType.Cancel)
                return;
        }

        {
            FileChooserDialog fcd = new FileChooserDialog(
                "Open File",
                this,
                FileChooserAction.Open,
                "Cancel", ResponseType.Cancel,
                "Open", ResponseType.Accept);
            fcd.Modal = true;
            fcd.DestroyWithParent = true;

            ResponseType resp = (ResponseType)fcd.Run();

            if (resp == ResponseType.Cancel)
            {
                fcd.Destroy();
                return;
            }

            filename = fcd.Filename;
            fcd.Destroy();

            switchTextDocument();
        }
    }

    protected void saveAction_Activated(object sender, EventArgs e)
    {
        if (filename == null)
        {
            FileChooserDialog fcd = new FileChooserDialog(
                "Save File",
                this,
                FileChooserAction.Save,
                "Cancel", ResponseType.Cancel,
                "Save", ResponseType.Accept);
            fcd.Modal = true;
            fcd.DestroyWithParent = true;

            ResponseType resp = (ResponseType)fcd.Run();

            if (resp == ResponseType.Cancel)
            {
                fcd.Destroy();
                return;
            }

            filename = fcd.Filename;
            fcd.Destroy();
        }
            
        File.WriteAllText(filename, editor.Document.Text);
        isEdited = false;
    }

    protected void saveAsAction_Activated(object sender, EventArgs e)
    {
        FileChooserDialog fcd = new FileChooserDialog(
            "Save File",
            this,
            FileChooserAction.Save,
            "Cancel", ResponseType.Cancel,
            "Save", ResponseType.Accept);
        fcd.Modal = true;
        fcd.DestroyWithParent = true;

        ResponseType resp = (ResponseType)fcd.Run();

        if (resp == ResponseType.Cancel)
        {
            fcd.Destroy();
            return;
        }

        filename = fcd.Filename;
        fcd.Destroy();

        File.WriteAllText(filename, editor.Document.Text);
        isEdited = false;
    }
    #endregion
}