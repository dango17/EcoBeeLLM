import sys
import torch
import torch.nn as nn
from transformers import AutoTokenizer, AutoModelForSeq2SeqLM

class HarmoniousCaramelModel(nn.Module):
    def __init__(self):
        super(HarmoniousCaramelModel, self).__init__()
        self.model = AutoModelForSeq2SeqLM.from_pretrained("neovalle/H4rmoniousCaramel")

    def forward(self, input_ids):
        return self.model.generate(input_ids, max_new_tokens=200)

def load_model(model_path):
    model = HarmoniousCaramelModel()
    model.load_state_dict(torch.load(model_path))
    model.eval()
    return model

def run_inference(model, tokenizer, prompt):
    inputs = tokenizer(prompt, return_tensors='pt')
    with torch.no_grad():
        output_ids = model(inputs["input_ids"])
        output = tokenizer.decode(output_ids[0], skip_special_tokens=True)
    return output

if __name__ == "__main__":
    model_path = sys.argv[1]
    prompt = " ".join(sys.argv[2:])
    tokenizer = AutoTokenizer.from_pretrained("neovalle/H4rmoniousCaramel")
    model = load_model(model_path)
    output = run_inference(model, tokenizer, prompt)
    print(output)