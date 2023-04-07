using AppNov14.Models.Base;
using AppNov14.Models.Comment;
using System;
using System.Collections.Generic;

namespace AppNov14.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        List<CommentModel> GetList(int status);

        MethodResult Create(CommentModel model);

        CommentModel GetComment(int id);

        MethodResult Close(int id, string answer);
    }
}