using AppNov14.Models.Base;
using AppNov14.Models.Comment;
using System;
using System.Collections.Generic;

namespace AppNov14.Handlers.Interfaces
{
    public interface ICommentHandler : IBaseDataHandler
    {
        List<CommentModel> GetList(int status);

        MethodResult Create(CommentModel model);

        CommentModel GetComment(int id);

        MethodResult Close(int id, string answer);
    }
}