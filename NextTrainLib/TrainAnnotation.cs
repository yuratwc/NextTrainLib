using System;
using System.Collections.Generic;
using System.Text;

namespace NextTrainLib
{
    public class TrainAnnotation
    {
        private char _label;
        private string _content;
        private string _comment;

        public char Label
        {
            get => _label;
            set => _label = value;
        }

        public string Content
        {
            get => _content; 
            set => _content = value;
        }

        public string Comment
        {
            get => _comment;
            set => _comment = value;
        }

        public TrainAnnotation()
        { }

        public TrainAnnotation(char label, string content)
            : this(label, content, string.Empty) { }

        public TrainAnnotation(char label, string content, string comment)
        {
            _label = label;
            _content = content;
            _comment = comment;
        }

        public override string ToString()
        {
            return $"Annotation({_label}:{_content}";
        }

        public override int GetHashCode()
        {
            return _label.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var annotation = obj as TrainAnnotation;
            if (annotation == null)
                return false;

            return annotation._label == _label && annotation._content == _content;
        }
    }
}
