abstract public class PopupPresenter : IPopupPresenter
{
    private readonly PopupModel _model;
    private readonly IPopupView _view;

    public PopupPresenter(PopupModel model, IPopupView view)
    {
        _model = model;
        _view = view;
    }

    public void ShowPopup()
    {
        _model.isVisible = true;
        _view.Show(_model.text);
    }

    public void HidePopup()
    {
        _model.isVisible = false;
        _view.Hide();
    }
}

